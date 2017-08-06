namespace ProductFileReader.Common.Entities
{
    public class Constants
    {
        public struct General
        {
            public const string ApplicationName             = "Product File Reader";
            public const string ConsolePrompt               = "command > ";
        }

        public struct RegexPatterns
        {
            public const string ArgumentPattern             = "<(.*?)>";
            public const string InputSplitPattern           = @"[<].+?[>]|[^ ]+";
        }

        public struct Commands
        {
            public const string CommandNamespace            = "ProductFileReader.Common.Commands";
            public const string CommandClassName            = "Commands";
            
        }

        public struct ErrorMessages
        {
            public const string FatalError                  = "An unexpected error occurred while processing your request.";
            public const string UnrecognizedCommandError    = "Unexpected exception occurred. Could not recognize command {0}.";
            public const string UnrecognizedCommand         = "Unknown command {0}. Please try again.";
            public const string MissingRequiredParam        = "Missing required argument {0}. Please try again.";
            public const string TryAgain                    = "Please try again.";
            public const string InvalidParamsFound          = "Invalid input argument found: ";
            public const string TooManyParams               = "Too many input arguments. Please try again.";
            public const string TypeParsingError            = "Could not parse the input argument {0}. Please try again.";
        }
    }
}
