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
            var methodParameters = classData.CommandMethodData[inputData.MethodName];
            if (methodParameters.Any())
            {
                methodParameters.ForEach(mp =>
                {
                    parameterValues.Add(mp.Name,mp.DefaultValue);
                });

                foreach (var methodParam in methodParameters)
                {
                    string inputDataValue;
                    if (inputData.Arguments.TryGetValue(methodParam.Name, out inputDataValue))
                    {
                        object value = null;
                        var paramType = methodParam.ParameterType;
                        value = ParseParameterValue(paramType, inputDataValue);
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
            object result = null;
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.String:
                    result = inputValue;
                    break;
                case TypeCode.Int32:
                    int numberValue;
                    inputValue = Regex.Match(inputValue, @"\d+").Value;
                    if (Int32.TryParse(inputValue, out numberValue))
                    {
                        result = numberValue;
                        break;
                    }
                    else
                    {
                        throw new InputException(string.Format(Constants.ErrorMessages.TypeParsingError, inputValue));
                    }
                case TypeCode.Boolean:
                    result = true;
                    break;
                default:
                    throw new InputException(string.Format(Constants.ErrorMessages.TypeParsingError, inputValue));
            }
            return result;
        }

  
    }
}
