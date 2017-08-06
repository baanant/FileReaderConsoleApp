using System.Collections.Generic;
using System.Reflection;

namespace ProductFileReader.Common.Entities
{
    public class CommandClassData
    {
        public CommandClassData(string name)
        {
            Name                = name;
            CommandMethodData   = new Dictionary<string, List<ParameterInfo>>();
        }

        public string Name { get; set; }

        public Dictionary<string, List<ParameterInfo>> CommandMethodData { get; set; }

        public void AddMethodParameterInfos(string methodName, ParameterInfo[] parameters)
        {
            if (CommandMethodData.ContainsKey(methodName))
            {
                CommandMethodData[methodName].AddRange(parameters);
            }
            else
            {
                var parameterInfos = new List<ParameterInfo>();
                parameterInfos.AddRange(parameters);
                CommandMethodData.Add(methodName, parameterInfos );
            }
        }

    }
}
