using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Utilities
{
    /// <summary>
    /// Executer class to execute commands with the input data.
    /// </summary>
    public static class CommandExecuter
    {

        /// <summary>
        /// Method to execute the command class method with the user input parameters.
        /// </summary>
        /// <param name="classData">Command class data.</param>
        /// <param name="inputData">User command input data.</param>
        /// <param name="assembly">Assembly.</param>
        /// <returns>String representation of the executed method.</returns>
        public static string ExecuteMethod(CommandClassData classData, CommandInputData inputData, Assembly assembly)
        {
            if(classData == null || assembly == null) throw new FatalException(Constants.ErrorMessages.FatalError);
            try
            {
                var parameterValues = SetParameterValues(classData, inputData);
                var classType       = assembly.GetType($"{Constants.Commands.CommandNamespace}.{classData.Name}");
                object[] methodParameterValues = null;
                if (parameterValues != null && parameterValues.Any())
                {
                    methodParameterValues = parameterValues.ToArray();
                }

                return classType.InvokeMember(inputData.MethodName, BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, methodParameterValues).ToString();
            }
            catch (InputException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new FatalException(Constants.ErrorMessages.FatalError);
            }
           
        }

        /// <summary>
        /// Sets the parameter values to a command method.
        /// </summary>
        /// <param name="classData"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private static IEnumerable<object> SetParameterValues(CommandClassData classData, CommandInputData inputData)
        {
            var parameterValues = new Dictionary<string, object>();
            //Get the command method data.
            var methodData      = classData.CommandMethodData.FirstOrDefault(cmd => cmd.MethodName == inputData.MethodName);
            if(methodData == null) throw new Exception();
            if (!methodData.Parameters.Any()) return null;
            //Set the method parameter default values.
            methodData.Parameters.ForEach(mp =>
            {
                parameterValues.Add(mp.Name,mp.DefaultValue);
            });

            //Try to set method parameter values.
            foreach (var methodParam in methodData.Parameters)
            {
                string inputDataValue;
                if (!inputData.Arguments.TryGetValue(methodParam.Name, out inputDataValue)) continue;
                var paramType   = methodParam.ParameterType;
                var value       = (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Nullable<>) && string.IsNullOrEmpty(inputDataValue)) ? null : ParseParameterValue(paramType, inputDataValue);
                parameterValues[methodParam.Name] = value;
            }
            return parameterValues.Values.ToList();
        }

        /// <summary>
        /// Parse input parameter value to a required type.
        /// ToDo: Add more supported types.
        /// </summary>
        /// <param name="type">Type of parameter.</param>
        /// <param name="inputValue">String input value.</param>
        /// <returns>Parsed parameter value.</returns>
        private static object ParseParameterValue(Type type, string inputValue)
        {
            var inputType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)
                ? Nullable.GetUnderlyingType(type)
                : type;
            if (inputType == typeof (string))
            {
                return inputValue;
            }
            if (inputType == typeof (int))
            {
                int numericValue;
                inputValue = Regex.Match(inputValue, Constants.RegexPatterns.NumericPattern).Value;
                if (int.TryParse(inputValue, out numericValue))
                {
                    return numericValue;
                }
                throw new InputException(string.Format(Constants.ErrorMessages.TypeParsingError, inputValue));
            }
            if (inputType == typeof (bool))
            {
                return true;
            }
            throw new InputException(string.Format(Constants.ErrorMessages.TypeParsingError, inputValue));
        }

  
    }
}
