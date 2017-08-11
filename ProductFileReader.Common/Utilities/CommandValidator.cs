using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Utilities
{
    public static class CommandValidator
    {
        public static void Validate(IEnumerable<CommandClassData> cmdClasses, CommandInputData cmdInput)
        {
            if (cmdClasses == null)
            {
                throw new InputException(string.Format(Constants.ErrorMessages.UnrecognizedCommandError, cmdInput.MethodName));
            }
            var correctClass = cmdClasses.FirstOrDefault(c => c.Name == cmdInput.ClassName);
            if (correctClass == null)
            {
                throw new InputException(string.Format(Constants.ErrorMessages.UnrecognizedCommandError, cmdInput.MethodName));
            }

            if (correctClass.CommandMethodData.All(cmd => cmd.MethodName != cmdInput.MethodName))
            {
                throw new InputException(string.Format(Constants.ErrorMessages.UnrecognizedCommand, cmdInput.MethodName));
            }
            var classMethod = correctClass.CommandMethodData.First(cmd => cmd.MethodName == cmdInput.MethodName);
            var classMethodParams = classMethod.Parameters;
            var requiredParams = classMethodParams.Where(p => !p.IsOptional).ToList();
            var optionalParams = classMethodParams.Where(p => p.IsOptional).ToList();

            cmdInput.Arguments.ToList().ForEach(a =>
            {
                if (classMethod.RequiredValueParams.Contains(a.Key) && string.IsNullOrEmpty(a.Value))
                {
                    throw new InputException(string.Format(Constants.ErrorMessages.ArgValueRequired, a.Key));
                }
            });

            if (cmdInput.Arguments.Count() > classMethodParams.Count)
            {
                throw new InputException(Constants.ErrorMessages.TooManyParams);
            }

            if (requiredParams.Any())
            {
                var missingRequiredParam = ValidateRequiredParams(requiredParams, cmdInput.Arguments);
                if (!string.IsNullOrEmpty(missingRequiredParam))
                {
                    throw new InputException(string.Format(Constants.ErrorMessages.MissingRequiredParam, missingRequiredParam));
                }
            }

            if (!requiredParams.Any() && !optionalParams.Any()) return;
            var invalidParameters = GetInvalidParams(optionalParams, requiredParams, cmdInput.Arguments);
            if (invalidParameters.Any())
            {
                var errorMsg = CreateInvalidParameterErrorMsg(invalidParameters);
                throw new InputException(errorMsg);
            }
        }

        private static string CreateInvalidParameterErrorMsg(IEnumerable<string> invalidParams)
        {
            var bob = new StringBuilder();
            bob.Append(Constants.ErrorMessages.InvalidParamsFound);
            foreach (var invalidParam in invalidParams)
            {
                bob.AppendFormat(invalidParams.Last() == invalidParam ? "{0}. " : "{0}, ", invalidParam);
            }
            bob.Append(Constants.ErrorMessages.TryAgain);
            return bob.ToString();
        }

        private static string ValidateRequiredParams(List<ParameterInfo> requiredMethodParams, IReadOnlyDictionary<string, string> inputParams)
        {
            var result = string.Empty;
            requiredMethodParams.ForEach(rp =>
            {
                if (!inputParams.ContainsKey(rp.Name.ToLower()))
                {
                    result = rp.Name;
                }
            });
            return result;
        }

        private static IEnumerable<string> GetInvalidParams(IEnumerable<ParameterInfo> optionalMethodParams, IEnumerable<ParameterInfo> requiredMethodParams, Dictionary<string, string> inputParams)
        {
            var result = new List<string>();
            foreach (var inputParam in inputParams)
            {
                if (!optionalMethodParams.Any(op => op.Name == inputParam.Key) &&
                    !requiredMethodParams.Any(rp => rp.Name == inputParam.Key))
                {
                    result.Add(inputParam.Key);
                }
            }
            return result;
        }

    }
}
