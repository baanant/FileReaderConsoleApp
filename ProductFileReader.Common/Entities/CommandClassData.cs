using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProductFileReader.Common.Entities
{
    /// <summary>
    /// CommandClassData class holds the reflected class data of existing commands.
    /// </summary>
    public class CommandClassData
    {
        public CommandClassData(string name)
        {
            Name                = name;
            CommandMethodData   = new List<CommandMethodData>();
        }

        public string Name { get; set; }


        public List<CommandMethodData> CommandMethodData { get; set; }

        public void AddMethodParameterInfos(string methodName, ParameterInfo[] parameters, IEnumerable<string> requiredValueParameters)
        {
            var existingCmd = CommandMethodData.FirstOrDefault(cmd => cmd.MethodName == methodName);
            if (existingCmd != null)
            {
                existingCmd.Parameters.AddRange(parameters);
                existingCmd.RequiredValueParams.ToList().AddRange(requiredValueParameters);
            }
            else
            {
                var newCmd = new CommandMethodData(methodName, parameters, requiredValueParameters);
                CommandMethodData.Add(newCmd);
            }
        }

    }
}
