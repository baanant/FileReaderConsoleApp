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
    /// <summary>
    /// Helper class to parse command information out of user input and existing command classes.
    /// </summary>
    public static class CommandParser
    {


        /// <summary>
        /// Parses user input to a CommandInputData object.
        /// </summary>
        /// <param name="inputCmdText"></param>
        /// <param name="cmdClassName"></param>
        /// <returns></returns>
        public static CommandInputData ParseInputData(string inputCmdText, string cmdClassName)
        {
            try
            {
                var result          = new CommandInputData(cmdClassName);
                var splitted        = SplitInputText(inputCmdText).ToList();
                var argumentName    = string.Empty;

                for (var i = 0; i < splitted.Count(); i++)
                {
                    var split = splitted[i].Trim();
                    if (i == 0) //Assume that the first input argument is always the command method. 
                    {
                        result.MethodName = split;
                    }
                    else
                    {
                        split = split.ToLower();
                        //Assume that the argument values are the text between less than & lt; and greater than & gt; characters.
                        //Assume that the previous input is the argument name.
                        if (IsArgumentValue(split, argumentName)) 
                        {
                            split = RemoveExtraChars(split);
                            result.Arguments.Add(argumentName, split);
                            argumentName = string.Empty;
                        }
                        //If argument name is not empty and it does not have value.
                        else if (!string.IsNullOrEmpty(argumentName))
                        {
                            result.Arguments.Add(argumentName, string.Empty);
                            argumentName = split;
                        }
                        else
                        {
                            //Set the argument name.
                            argumentName = split;
                        }

                        //If last split is not empty add it as an argument.
                        if (i == splitted.Count() - 1 && !string.IsNullOrEmpty(argumentName)) result.Arguments.Add(argumentName, string.Empty);
                    }

                }
                return result;
            }
            catch (Exception)
            {
                throw new FatalException(Constants.ErrorMessages.FatalError);
            }

        }


        /// <summary>
        /// Remove extra characters from argument value input.
        /// Todo: Create a proper Regex.
        /// </summary>
        /// <param name="inputSplit"></param>
        /// <returns>Argument value</returns>
        private static string RemoveExtraChars(string inputSplit)
        {
            var result = inputSplit.Replace(">", string.Empty).Replace("<", string.Empty).Replace("\\", @"\").Replace("//", "/");
            return result;
        }


        /// <summary>
        /// Take the input and check if the input text is argument value.
        /// Assume that the argument values are the text between less than &lt; and greater than &gt; characters.
        /// ToDo! Make this to work with tabs as well.
        /// </summary>
        /// <param name="inputSplit"></param>
        /// <param name="argumentName"></param>
        /// <returns>True if input is considered as argument value.</returns>
        private static bool IsArgumentValue(string inputSplit, string argumentName)
        {
            return Regex.IsMatch(inputSplit, Constants.RegexPatterns.ArgumentPattern) && !string.IsNullOrEmpty(argumentName);
        }



        /// <summary>
        /// Method to split the input text in splits.
        /// </summary>
        /// <param name="inputCmdText">Input command text.</param>
        /// <returns></returns>
        private static IEnumerable<string> SplitInputText(string inputCmdText)
        {
            return Regex.Matches(inputCmdText, Constants.RegexPatterns.InputSplitPattern)
                .Cast<Match>()
                .Select(m => m.Value);
        }

        /// <summary>
        /// Reads the existing command class data.
        /// </summary>
        /// <param name="cmdNamespace">Command classes namespace.</param>
        /// <returns>Parsed object reflection information.</returns>
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
