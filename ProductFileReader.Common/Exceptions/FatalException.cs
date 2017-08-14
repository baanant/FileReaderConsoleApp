using System;

namespace ProductFileReader.Common.Exceptions
{
    /// <summary>
    /// Fatal exception class.
    /// </summary>
    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
            GetInnerExceptionMessages(InnerException);
        }

        public string InnerExceptionMessages { get; private set; }

        /// <summary>
        /// Go deep.
        /// </summary>
        /// <param name="ex">Exception.</param>
        private void GetInnerExceptionMessages(Exception ex)
        {
            while (true)
            {
                if (ex == null || ex.InnerException == null) return;
                InnerExceptionMessages = $"{InnerExceptionMessages} \n {ex.InnerException.Message}";
                ex = ex.InnerException.InnerException;
            }
        }
    }
}
