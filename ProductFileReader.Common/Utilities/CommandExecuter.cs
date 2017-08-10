using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Utilities
{
    public static class CommandExecuter
    {
        public static string ExecuteMethod(CommandClassData classData, CommandInputData inputData, Assembly assembly)
        {
            if(classData == null || assembly == null) throw new FatalException(Constants.ErrorMessages.FatalError);
            try
            {
                var parameterValues = SetParameterValues(classData, inputData);
                Type classType = assembly.GetType($"{Constants.Commands.CommandNamespace}.{classData.Name}");
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

        private static List<object> SetParameterValues(CommandClassData classData, CommandInputData inputData)
        {
            var parameterValues = new Dictionary<string, object>();
            var methodData = classData.CommandMethodData.FirstOrDefault(cmd => cmd.MethodName == inputData.MethodName);
            if(methodData == null) throw new Exception();
            if (methodData.Parameters.Any())
            {
                methodData.Parameters.ForEach(mp =>
                {
                    parameterValues.Add(mp.Name,mp.DefaultValue);
                });


                foreach (var methodParam in methodData.Parameters)
                {
                    string inputDataValue;
                    if (inputData.Arguments.TryGetValue(methodParam.Name, out inputDataValue))
                    {
                        var paramType = methodParam.ParameterType;
                        var value = (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Nullable<>) && string.IsNullOrEmpty(inputDataValue)) ? null : ParseParameterValue(paramType, inputDataValue);
                        parameterValues[methodParam.Name] = value;
                    }

                }
                return parameterValues.Values.ToList();
            }
            return null;
        }

        //ToDo: Add more supported types.
        private static object ParseParameterValue(Type type, string inputValue)
        {
            var inputType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)
                ? Nullable.GetUnderlyingType(type)
                : type;
            if (inputType == typeof (string))
            {
                return inputValue;
            }
            else if (inputType == typeof (int))
            {
                int numericValue;
                inputValue = Regex.Match(inputValue, Constants.RegexPatterns.NumericPattern).Value;
                if (int.TryParse(inputValue, out numericValue))
                {
                    return numericValue;
                }
                throw new InputException(string.Format(Constants.ErrorMessages.TypeParsingError, inputValue));
            }
            else if (inputType == typeof (bool))
            {
                return true;
            }
            throw new InputException(string.Format(Constants.ErrorMessages.TypeParsingError, inputValue));
        }

  
    }
}
