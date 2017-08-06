using System.Collections.Generic;

namespace ProductFileReader.Common.Entities
{
    public class CommandInputData
    {
        public CommandInputData(string className)
        {
            ClassName       = className;
            Arguments       = new Dictionary<string, string>();
        }

        public string MethodName { get; set; }

        public string ClassName { get; set; }

        public Dictionary<string, string> Arguments { get; set; } 
    }
}
