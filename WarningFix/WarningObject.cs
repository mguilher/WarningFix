using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarningFix
{
    public class WarningObject
    {
        public string Message { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int StartLineNumber { get; set; }
        public int StartColumnNumber { get; set; }
        public int EndLineNumber { get; set; }
        public int EndColumnNumber { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
