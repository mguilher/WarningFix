using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarningFix
{
    public class WarningParser
    {
        private readonly ILogger<WarningParser> _logger;
        private Dictionary<string, int> _warningStatistics = new Dictionary<string, int>();
        private List<WarningObject> warningsSamples = new List<WarningObject>();

        private WarningObject GetTypeCS(string warningLine)
        {
            var parts = warningLine.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3)
            {
                var warning = new WarningObject
                {
                    FilePath = parts[0].Trim(),
                    Message = parts[2].Trim()
                };

                int indexWarning = warning.Message.IndexOf("warning");
                if (indexWarning >= 0)
                {
                    string warningCode = warning.Message.Substring(indexWarning, 20);
                    if (warningCode.Contains(':'))
                        warningCode = warningCode.Substring(0, warningCode.IndexOf(':')).Trim();
                    warning.Code = warningCode.Replace("warning", "").Trim();
                    if (_warningStatistics.ContainsKey(warning.Code))
                    {
                        _warningStatistics[warning.Code]++;
                    }
                    else
                    {
                        _warningStatistics[warning.Code] = 1;
                    }
                }

                var positionData = parts[1].Trim();

                if (positionData.Contains(','))
                {
                    var innerParts = positionData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (innerParts.Length == 4)
                    {
                        warning.StartLineNumber = int.Parse(innerParts[0]);
                        warning.StartColumnNumber = int.Parse(innerParts[1]);
                        warning.EndLineNumber = int.Parse(innerParts[2]);
                        warning.EndColumnNumber = int.Parse(innerParts[3]);
                    }
                }

                return warning;
            }

            return new WarningObject();
        }


        private WarningObject GetTypeNU(string beforeWarning, string afterWarning)
        {
            if (!string.IsNullOrEmpty(beforeWarning) && !string.IsNullOrEmpty(afterWarning))
            {
                var warning = new WarningObject
                {
                    FilePath = beforeWarning.Replace(":", "").Trim(),
                    Message = afterWarning.Trim()
                };

                int indexWarning = warning.Message.IndexOf("warning");
                if (indexWarning >= 0)
                {
                    string warningCode = warning.Message.Substring(indexWarning, 20);
                    if (warningCode.Contains(':'))
                        warningCode = warningCode.Substring(0, warningCode.IndexOf(':')).Trim();
                    warning.Code = warningCode.Replace("warning", "").Trim();
                    if (_warningStatistics.ContainsKey(warning.Code))
                    {
                        _warningStatistics[warning.Code]++;
                    }
                    else
                    {
                        _warningStatistics[warning.Code] = 1;
                    }
                }

                return warning;
            }

            return new WarningObject();
        }

        public WarningParser(ILogger<WarningParser> logger)
        {
            _logger = logger;
        }
        public List<WarningObject> ParseWarnings(string logContent)
        {
            var warnings = new List<WarningObject>();
            var lines = logContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                // Example log line format:
                // 16>D:\GitHub\Backend\MyApi\Service.cs(110,36,110,60): warning CS8601: Possible null reference assignment.
                if (line.Contains("warning"))
                {
                    int startIndex = line.IndexOf('>') + 1;
                    var warningLine = line.Substring(startIndex);

                    int warningIndex = warningLine.IndexOf("warning");
                    string beforeWarning = warningLine.Substring(0, warningIndex);
                    string afterWarning = warningLine.Substring(warningIndex);

                    if (beforeWarning.Contains('(') && beforeWarning.Contains(')'))
                    {
                        var warning = GetTypeCS(warningLine);
                        if (!string.IsNullOrEmpty(warning.Message))
                        {
                            warnings.Add(warning);
                        }
                    }
                    else
                    {
                        var warning = GetTypeNU(beforeWarning, afterWarning);
                        if (!string.IsNullOrEmpty(warning.Message))
                        {
                            warnings.Add(warning);
                        }
                        else
                        {
                            _logger.LogInformation("Unrecognized warning format: {Line}", line);
                        }
                    }

                }
            }



            foreach (var kvp in _warningStatistics)
            {
                _logger.LogInformation("Warning {Code}: {Count} occurrences", kvp.Key, kvp.Value);
                var topWarnings = warnings.Where(w => w.Code == kvp.Key).Take(3);
                warningsSamples.AddRange(topWarnings);
            }

            return warnings;
        }

        public void PrintWarningStatistics(bool withSamples)
        {
            _logger.LogInformation("Warning Statistics:");

            foreach (var kvp in _warningStatistics)
            {
                _logger.LogInformation("Warning {Code}: {Count} occurrences", kvp.Key, kvp.Value);
                if (withSamples)
                {
                    foreach (var warning in warningsSamples.Where(w => w.Code == kvp.Key))
                    {
                        _logger.LogInformation("");
                        _logger.LogInformation("Message:{Message}", warning.Message);
                        _logger.LogInformation("FilePath:{FilePath}", warning.FilePath);
                        _logger.LogInformation("StartLineNumber:{StartLine} ColumnNumber:{StartColumn} EndLineNumber:{EndLine} EndColumnNumber:{EndColumn}", warning.StartLineNumber, warning.StartColumnNumber, warning.EndLineNumber, warning.EndColumnNumber);

                    }
                }
               
                _logger.LogInformation("--------------------------------------------------");
            }
        }
    }
}
