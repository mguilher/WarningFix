// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using WarningFix;
using WarningFix.Agent;
using WarningFix.Copilot;


using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(options =>
    {
        
    });
});

var agentLogger = loggerFactory.CreateLogger<AgentWarning>();
var parserLogger = loggerFactory.CreateLogger<WarningParser>();
var promptLogger = loggerFactory.CreateLogger<CreatePrompt>();
var copilotLogger = loggerFactory.CreateLogger<CopilotAgentWarning>();

Console.WriteLine("Start");

string input = string.Empty;
bool neddReadFromFile = true;
if (args.Length > 0)
{
    if (!string.IsNullOrEmpty(args[0]))
    {
        if (File.Exists(args[0]))
        {
            input = File.ReadAllText(args[0]);
            neddReadFromFile = false;
        }
        else
        {
            Console.WriteLine("File not found");
        }
    }
}
if (neddReadFromFile)
{
    Console.WriteLine("Enter Warning lines file path");
    string fileName = Console.ReadLine();
    if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
    {
        input = File.ReadAllText(fileName);
    }
    else
    {
        Console.WriteLine("File not found");
    }
}

if (!string.IsNullOrEmpty(input))
{
    var parser = new WarningParser(parserLogger);
    List<WarningObject> warnings = parser.ParseWarnings(input);

    Console.WriteLine($"Parsed {warnings.Count} warnings");
    parser.PrintWarningStatistics(false);

    CreatePrompt createPrompt = new(promptLogger);
    List<string> prompts = createPrompt.CreatePrompts(warnings.Where(w => !string.IsNullOrEmpty(w.Message) && w.Code.StartsWith("CS")).ToList());

    //File.WriteAllText("prompts.txt", string.Join(Environment.NewLine + "-----" + Environment.NewLine, prompts));

    //var endpoint = "https://models.github.ai/inference";
    //var model = "openai/gpt-5-mini";
    //var githubkey = Environment.GetEnvironmentVariables()["GITHUB_API_KEY"]?.ToString() ?? throw new ArgumentNullException("GITHUB_API_KEY environment variable is not set");
    //AgentWarning agentWarning = new(agentLogger, githubkey, model, endpoint);
    //await agentWarning.RunFixWarningsAsync(prompts, recordAgentThread: false);

    CopilotAgentWarning copilotAgentWarning = new(copilotLogger);
    await copilotAgentWarning.RunFixWarningsAsync(prompts);
}
