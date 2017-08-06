using System;

namespace ProductFileReader.Common.Exceptions
{
    public class FatalException : Exception
    {
        string _innerExceptionMsg;
        public FatalException(string message) : base(message)
        {
            GetInnerExceptionMessages(InnerException);
        }

        public string InnerExceptionMessages { get { return _innerExceptionMsg; } }

        private void GetInnerExceptionMessages(Exception ex)
        {
            if (ex != null && ex.InnerException != null)
            {
                _innerExceptionMsg = $"{_innerExceptionMsg} \n {ex.InnerException.Message}";
                GetInnerExceptionMessages(ex.InnerException.InnerException);
            }
        }


    }
}
