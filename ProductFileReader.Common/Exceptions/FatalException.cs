using System;

namespace ProductFileReader.Common.Exceptions
{
    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
            GetInnerExceptionMessages(InnerException);
        }

        public string InnerExceptionMessages { get; private set; }

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
