using GitHub.Copilot.SDK;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WarningFix.Agent.Tools;

namespace WarningFix.Copilot
{
    public class CopilotAgentWarning
    {
        private readonly ILogger<CopilotAgentWarning> _logger;

        public CopilotAgentWarning(ILogger<CopilotAgentWarning> logger)
        {
            _logger = logger;
        }

        private Action _taskDone;

        public async Task<bool> RunFixWarningsAsync(List<string> prompts)
        {
            try
            {
                var client = new CopilotClient();
                await client.StartAsync();

                var session = await client.CreateSessionAsync(new SessionConfig
                {
                    Model = "gpt-4.1",
                });

                session.On(evt =>
                {
                    switch (evt)
                    {
                        case AssistantMessageDeltaEvent delta:
                            _logger.LogInformation("{DeltaContent}", delta.Data.DeltaContent);
                            break;
                        case AssistantReasoningDeltaEvent reasoningDelta:
                            _logger.LogInformation("{DeltaContent}", reasoningDelta.Data.DeltaContent);
                            break;
                        case AssistantMessageEvent msg:
                            _logger.LogInformation("\n--- Final message ---");
                            _logger.LogInformation("{Content}", msg.Data.Content);
                            break;
                        case AssistantReasoningEvent reasoningEvt:
                            _logger.LogInformation("--- Reasoning ---");
                            _logger.LogInformation("{Content}", reasoningEvt.Data.Content);
                            break;
                        case SessionIdleEvent sessionIdleEvent:
                            _logger.LogInformation("Session is idle.{Id}", sessionIdleEvent.Id);
                            _taskDone?.Invoke();
                            break;
                    }
                });


                foreach (var prompt in prompts)
                {
                    _logger.LogInformation("Processing prompt: {Prompt}", prompt);

                    // Create a new TaskCompletionSource for each request
                    var done = new TaskCompletionSource();                   
                    _taskDone = () => done.TrySetResult();

                    // Send the actual prompt (not the hardcoded test message)
                    var result = await session.SendAsync(new MessageOptions { Prompt = prompt });
                    
                    // Wait for session to become idle before continuing
                    await done.Task;

                    _logger.LogInformation("{Result}", result?.ToString() ?? "No result");
                }

                // Dispose resources after all processing is complete
                await session.DisposeAsync();
                await client.DisposeAsync();
            }
            catch (StreamJsonRpc.RemoteInvocationException ex)
            {
                _logger.LogError("JSON-RPC Error: {Error}", ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {Error}", ex.ToString());
                return false;
            }

            return true;
        }
    }
}
