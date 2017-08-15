using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;
using ProductFileReader.Common.Utilities;

namespace ProductFileReader.UnitTests
{
    [TestFixture]
    public class CommandValidatorTests
    {
        private IEnumerable<CommandClassData>   _classData;
        private CommandInputData                _cmdInputDataValid;
        private CommandInputData                _cmdInputDataInvalidClassName;
        private CommandInputData                _cmdInputDataInvalidMethodName;
        private CommandInputData                _cmdInputDataTooManyArguments;
        private CommandInputData                _cmdInputDataArgumentValueMissing;
        private CommandInputData                _cmdInputDataRequiredArgumentMissing;
        private CommandInputData                _cmdInputDataInvalidArgument;


        [OneTimeSetUp]
        public void Init()
        {
            _classData                              = CommandParser.ParseClassData(Constants.Commands.CommandNamespace);
                
            var arguments                           = new Dictionary<string, string>()
            {
                { "file", "test" },
                { "project", "1" },
                { "sortbystartdate", string.Empty }

            };
            _cmdInputDataValid                      = new CommandInputData("Commands") { MethodName = "Read", Arguments = arguments };

            _cmdInputDataInvalidClassName           = new CommandInputData("Commando");

            _cmdInputDataInvalidMethodName          = new CommandInputData("Commands") { MethodName = "Write" };

            var argumentsTwo                        = new Dictionary<string, string>
            {
                {"file", "test"},
                {"project", "1"},
                {"test", "test"},
                {"test2", "test2"}
            };
            _cmdInputDataTooManyArguments           = new CommandInputData("Commands") { MethodName = "Read", Arguments = argumentsTwo };

            var argumentsThree                      = new Dictionary<string, string> {{"file", string.Empty}};
            _cmdInputDataArgumentValueMissing       = new CommandInputData("Commands") { MethodName = "Read", Arguments = argumentsThree };

            var argumentsFour                       = new Dictionary<string, string> {{"SortByStartDate", string.Empty}};
            _cmdInputDataRequiredArgumentMissing    = new CommandInputData("Commands") { MethodName = "Read", Arguments = argumentsFour };


            var argumentsFive                       = new Dictionary<string, string>()
            {
                {"file", "test"},
                {"project", "1"},
                {"test", "test"}
            };
            _cmdInputDataInvalidArgument            = new CommandInputData("Commands") { MethodName = "Read", Arguments = argumentsFive };
            
        }


        [Test]
        public void ValidateShouldBeValid()
        {
            CommandValidator.Validate(_classData, _cmdInputDataValid);
        }

        [Test]
        public void ValidateShouldThrowInputExceptionBecauseOfRequiredArgumentMissing()
        {
            Assert.Throws<InputException>(() => CommandValidator.Validate(_classData, _cmdInputDataRequiredArgumentMissing), "Because of required argument missing.");
        }

        [Test]
        public void ValidateShouldThrowInputExceptionBecauseOfInvalidArgument()
        {
            Assert.Throws<InputException>(() => CommandValidator.Validate(_classData, _cmdInputDataInvalidArgument), "Because of invalid argument.");
        }

        [Test]
        public void ValidateShouldThrowInputExceptionBecauseOfInvalidClassName()
        {
            var ex = Assert.Throws<InputException>(() => CommandValidator.Validate(_classData, _cmdInputDataInvalidClassName), "Because of invalid class name.");
            ex.Message.ShouldBeEquivalentTo(string.Format(Constants.ErrorMessages.UnrecognizedCommandError, _cmdInputDataInvalidClassName.MethodName));
        }

        [Test]
        public void ValidateShouldThrowInputExceptionBecauseOfInvalidMethodName()
        {
            var ex = Assert.Throws<InputException>(() => CommandValidator.Validate(_classData, _cmdInputDataInvalidMethodName), "Because of invalid method name.");
            ex.Message.ShouldBeEquivalentTo(string.Format(Constants.ErrorMessages.UnrecognizedCommand, _cmdInputDataInvalidMethodName.MethodName));
        }

        [Test]
        public void ValidateShouldThrowInputExceptionBecauseOfArgumentValueMissing()
        {
            Assert.Throws<InputException>(() => CommandValidator.Validate(_classData, _cmdInputDataArgumentValueMissing), "Because of argument value is missing.");
        }

        [Test]
        public void ValidateShouldThrowInputExceptionBecauseOfTooManyArguments()
        {
            var ex = Assert.Throws<InputException>(() => CommandValidator.Validate(_classData, _cmdInputDataTooManyArguments), "Because of too many arguments.");
            ex.Message.ShouldBeEquivalentTo(Constants.ErrorMessages.TooManyParams);
        }

    }
}
