using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProductFileReader.Common.Entities
{
    /// <summary>
    /// A class to hold the method data for existing commands.
    /// </summary>
    public class CommandMethodData
    {
        public CommandMethodData(string name, IEnumerable<ParameterInfo> parameters,
            IEnumerable<string> requiredValueParams)
        {
            MethodName          = name;
            Parameters          = parameters.ToList();
            RequiredValueParams = requiredValueParams.ToList();
        }

        public string MethodName { get; set; }

        public List<ParameterInfo> Parameters { get; set; }
        
        public List<string> RequiredValueParams { get; set; } 
    }
}
