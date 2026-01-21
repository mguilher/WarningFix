using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WarningFix.Agent.Tools
{
    [PublicAPI]
    public class FileSystemTools
    {
        public void EditFile(string filePath, string newContent)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }
            File.WriteAllText(filePath, newContent);
        }

        public void InsertLineAt(string filePath, int lineNumber, string content)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }

            var lines = File.ReadAllLines(filePath).ToList();

            if (lineNumber < 0 || lineNumber > lines.Count)
            {
                throw new Exception($"Line number must be between 0 and {lines.Count}");
            }

            lines.Insert(lineNumber, content);
            File.WriteAllLines(filePath, lines);
        }

        public void InsertLinesAt(string filePath, int lineNumber, string[] contentLines)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }

            var lines = File.ReadAllLines(filePath).ToList();

            if (lineNumber < 0 || lineNumber > lines.Count)
            {
                throw new Exception($"Line number must be between 0 and {lines.Count}");
            }

            lines.InsertRange(lineNumber, contentLines);
            File.WriteAllLines(filePath, lines);
        }

        public void RemoveLineAt(string filePath, int lineNumber)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }

            var lines = File.ReadAllLines(filePath).ToList();

            if (lineNumber < 0 || lineNumber >= lines.Count)
            {
                throw new Exception($"Line number must be between 0 and {lines.Count - 1}");
            }

            lines.RemoveAt(lineNumber);
            File.WriteAllLines(filePath, lines);
        }

        public void RemoveLinesAt(string filePath, int startLineNumber, int count)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }

            var lines = File.ReadAllLines(filePath).ToList();

            if (startLineNumber < 0 || startLineNumber >= lines.Count)
            {
                throw new Exception($"Start line number must be between 0 and {lines.Count - 1}");
            }

            if (count < 1)
            {
                throw new Exception("Count must be at least 1");
            }

            if (startLineNumber + count > lines.Count)
            {
                throw new Exception($"Cannot remove {count} lines starting at line {startLineNumber}. File only has {lines.Count} lines.");
            }

            lines.RemoveRange(startLineNumber, count);
            File.WriteAllLines(filePath, lines);
        }

        public void ReplaceLineAt(string filePath, int lineNumber, string newContent)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }

            var lines = File.ReadAllLines(filePath).ToList();

            if (lineNumber < 0 || lineNumber >= lines.Count)
            {
                throw new Exception($"Line number must be between 0 and {lines.Count - 1}");
            }

            lines[lineNumber] = newContent;
            File.WriteAllLines(filePath, lines);
        }

        public string[] GetFileLines(string filePath)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }
            return File.ReadAllLines(filePath);
        }

        public int GetLineCount(string filePath)
        {
            Guard(filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist");
            }
            return File.ReadAllLines(filePath).Length;
        }

        private void Guard(string folderPath)
        {
            //if (!folderPath.StartsWith(RootFolder))
            //{
            //    throw new Exception("No you don't!");
            //}
        }
    }
}
