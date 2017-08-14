using System;

namespace ProductFileReader.Common.Exceptions
{
    /// <summary>
    /// Input exception class.
    /// </summary>
    public class InputException: Exception
    {
        public InputException(string message) : base(message) { }
    }
}
