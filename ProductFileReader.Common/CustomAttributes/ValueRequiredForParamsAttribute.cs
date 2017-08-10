using System;
using System.Collections.Generic;

namespace ProductFileReader.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class ValueRequiredForParamsAttribute : Attribute
    {

        public ValueRequiredForParamsAttribute(params string[] parameters)
        {
            Parameters = parameters;
        }

        public IEnumerable<string> Parameters { get; }
    }
}