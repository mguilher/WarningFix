using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarningFix
{
    public class CreatePrompt
    {
        private readonly ILogger<CreatePrompt> _logger;

        public CreatePrompt(ILogger<CreatePrompt> logger)
        {
            _logger = logger;
        }

        public List<string> CreatePrompts(List<WarningObject> warnings)
        {
            List<string> prompts = new List<string>();
            // Group warnings by file path
            var warningsByFile = warnings.GroupBy(w => w.FilePath);

            foreach (var fileGroup in warningsByFile)
            {
                var fileWarnings = fileGroup.ToList();
                _logger.LogInformation("Processing {Count} warning(s) in file: {FilePath}", fileWarnings.Count, fileGroup.Key);

                StringBuilder promptBuilder = new();
                promptBuilder.AppendLine($"Fix the following {fileWarnings.Count} warning(s) in the file '{fileGroup.Key}':");
                promptBuilder.AppendLine();

                var codeWarnings = fileWarnings.GroupBy(w => w.Code);

                foreach (var codeWarning in codeWarnings)
                {
                    var groupWarnings = codeWarning.ToList();

                    promptBuilder.AppendLine($"- Code:{codeWarning.Key}");

                    foreach (var warning in groupWarnings)
                    {
                        promptBuilder.AppendLine($"Message:{warning.Message}; Line:{warning.StartLineNumber}; Column:{warning.StartColumnNumber}");
                       
                    }

                    string instruction = codeWarning.Key switch
                    {
                        "CS0105" => "Duplicate using directive. Remove the duplicate using statement.",
                        "CS0108" => "Member hides inherited member. Add 'new' keyword if hiding is intentional, or use 'override' to override the base member.",
                        "CS0168" => "Variable declared but never used. Remove the variable, use discard '_', or actually use the variable.",
                        "CS8073" => "Result of expression is always the same. Remove or fix the redundant comparison logic.",
                        "CS8600" => "Converting possible null value to non-nullable type.  Use safe casting with null-coalescing:  '(obj as string) ?? string.Empty' or '(obj as ClassName) ?? new ClassName()'.",
                        "CS8601" => "Possible null reference assignment. Use null-coalescing operator: for strings use '?? string.Empty', for objects use '? ? new ClassName()'.",
                        "CS8602" => "Dereference of possibly null reference.  Use null-coalescing operator:  for strings use '? ?  string.Empty', for objects use '?? new ClassName()'.",
                        "CS8603" => "Possible null reference return. Change return type to nullable '?' ",
                        "CS8604" => "Possible null reference argument.  Use null-coalescing operator:  for strings use '?? string.Empty', for objects use '?'.",
                        "CS8618" => "Non-nullable property not initialized.  For strings set default to 'string.Empty', for objects set default to 'new ClassName()', for collections use 'new List<T>()' or 'Array.Empty<T>()'.",
                        "CS8619" => "Nullability mismatch in value.  Align nullability or change return type to nullable '?'",
                        "CS8625" => "Cannot convert null to non-nullable type. Replace null with default value: for strings use 'string.Empty', for objects use 'new ClassName()' checking the constructor.",
                        "CS8629" => "Nullable value type may be null. Use '??  defaultValue' or '.GetValueOrDefault()' instead of '. Value'.",
                        "CS8632" => "Missing nullable reference type annotation. Add '?' to the type to indicate it can be null.",
                        "CS8765" => "Nullability mismatch in return type. Align nullability or change return type to nullable '?'",
                        _ => string.Empty
                    };
                    if (!string.IsNullOrEmpty(instruction))
                    {
                        promptBuilder.AppendLine($"  Instruction: {instruction}");
                    }

                }

                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Use the file editing tools to apply all fixes in a single operation.");

                string prompt = promptBuilder.ToString();
                prompts.Add(prompt);
            }
            return prompts;
        }
    }
}
