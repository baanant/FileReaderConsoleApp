using System;
using System.Collections.Generic;

namespace ProductFileReader.Common.CustomAttributes
{

    /// <summary>
    /// An attribute class to contain the information which command method parameters requires values. 
    /// ToDo! Find a way to do this differently.
    /// </summary>
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