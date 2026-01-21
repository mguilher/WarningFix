using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using WarningFix.Agent.Tools;

namespace WarningFix.Agent
{
    public class AgentWarning
    {
        private readonly ILogger<AgentWarning> _logger;

        private string _apiKey;
        private string _model;
        private string _endpoint;

        public AgentWarning(ILogger<AgentWarning> logger, string apiKey, string model, string endpoint)
        {
            _logger = logger;
            _apiKey = apiKey;
            _model = model;
            _endpoint = endpoint;
        }

        async ValueTask<object?> FunctionCallMiddleware(AIAgent callingAgent, FunctionInvocationContext context, Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
        {
            StringBuilder functionCallDetails = new();
            functionCallDetails.Append($"- Tool Call: '{context.Function.Name}'");
            if (context.Arguments.Count > 0)
            {
                functionCallDetails.Append($" (Args: {string.Join(",", context.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))}");
            }

            _logger.LogInformation("{FunctionCallDetails}", functionCallDetails.ToString());

            return await next(context, cancellationToken);
        }

        public async Task<string> RunFixWarningsAsync(List<string> prompts, bool recordAgentThread)
        {
            var endpoint = new Uri(_endpoint);
            var model = _model;
            var githubkey = _apiKey;

            var openAIOptions = new OpenAIClientOptions()
            {
                Endpoint = endpoint,
            };


            FileSystemTools target = new();
            MethodInfo[] methods = typeof(FileSystemTools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<AITool> listOfTools = methods.Select(x => AIFunctionFactory.Create(x, target)).Cast<AITool>().ToList();

            AIAgent agent = new ChatClient(model, new ApiKeyCredential(githubkey), openAIOptions)
                                .CreateAIAgent(instructions: "You are a DotNet c# expert", name: "AgentWarning", tools: listOfTools)
                                .AsBuilder().Use(FunctionCallMiddleware)
                                .UseOpenTelemetry(sourceName: "agent-telemetry-source")
                                .Build();

            AgentThread agentThread = agent.GetNewThread();

            foreach (var prompt in prompts)
            {
                _logger.LogInformation("Generated prompt:\n{Prompt}", prompt);

                var result = await agent.RunAsync(prompt, agentThread);
                _logger.LogInformation("{Result}", result.ToString());
            }

            if (recordAgentThread)
            {
                _logger.LogInformation("Recording agent thread...");
                string serializedConversation = agentThread.Serialize(JsonSerializerOptions.Web).GetRawText();
                string identifier = DateTime.Now.ToString("yyyyMMddHHmmss");
                string filePath = Path.Combine(Path.GetTempPath(), $"conversation_{identifier}.json");
                await File.WriteAllTextAsync(filePath, serializedConversation);
                _logger.LogInformation("Agent thread recorded to {FilePath}", filePath);
            }

            return "AgentWarning: ";
        }
    }
}
