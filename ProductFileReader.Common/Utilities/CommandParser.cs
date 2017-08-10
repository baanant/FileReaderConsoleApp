using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ProductFileReader.Common.CustomAttributes;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Utilities
{
    public static class CommandParser
    {

        public static CommandInputData ParseInputData(string inputCmdText, string cmdClassName)
        {
            try
            {
                var result          = new CommandInputData(cmdClassName);
                var splitted        = SplitInputText(inputCmdText);
                var argumentName    = string.Empty;

                for (var i = 0; i < splitted.Count(); i++)
                {
                    var split = splitted[i].Trim();
                    if (i == 0)
                    {
                        result.MethodName = split;
                    }
                    else
                    {
                        split = split.ToLower();
                        if (IsArgumentValue(split, argumentName))
                        {
                            split = RemoveExtraChars(split);
                            result.Arguments.Add(argumentName, split);
                            argumentName = string.Empty;
                        }
                        else if (!string.IsNullOrEmpty(argumentName))
                        {
                            result.Arguments.Add(argumentName, string.Empty);
                            argumentName = split;
                        }
                        else
                        {
                            argumentName = split;
                            if (i == splitted.Count() - 1) result.Arguments.Add(argumentName, string.Empty);
                        }
                    }

                }
                return result;
            }
            catch (Exception)
            {
                throw new FatalException(Constants.ErrorMessages.FatalError);
            }

        }

        //Todo: Create a proper Regex.
        private static string RemoveExtraChars(string inputSplit)
        {
            var result = inputSplit.Replace(">", string.Empty).Replace("<", string.Empty).Replace("\\", @"\").Replace("//", "/");
            return result;
        }

        //ToDo! Make this to work with tabs as well.
        private static bool IsArgumentValue(string inputSplit, string argumentName)
        {
            return Regex.IsMatch(inputSplit, Constants.RegexPatterns.ArgumentPattern) && !string.IsNullOrEmpty(argumentName);
        }




        private static List<string> SplitInputText(string inputCmdText)
        {
            return Regex.Matches(inputCmdText, Constants.RegexPatterns.InputSplitPattern)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();
        }

        public static IEnumerable<CommandClassData> ParseClassData(string cmdNamespace)
        {
            try
            {
                var result = new List<CommandClassData>();
                var cmdClasses =
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.IsClass && t.Namespace == cmdNamespace)
                        .ToList();
                cmdClasses.ForEach(c =>
                {
                    var cmdClassData = new CommandClassData(c.Name);
                    var cmdClassMethods = c.GetMethods(BindingFlags.Static | BindingFlags.Public).ToList();
                    cmdClassMethods.ForEach(cm =>
                    {
                        var requiredValueParams = cm.GetCustomAttribute<ValueRequiredForParamsAttribute>().Parameters;
                        cmdClassData.AddMethodParameterInfos(cm.Name, cm.GetParameters(), requiredValueParams);
                        

                    });
                    result.Add(cmdClassData);
                });
                return result;
            }
            catch (Exception)
            {
                throw new FatalException(Constants.ErrorMessages.FatalError);
            }
        }
    }
}
