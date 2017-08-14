using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using ProductFileReader.Common.Utilities;

namespace ProductFileReader.UnitTests
{
    [TestFixture]
    public class CommandParserTests
    {
        PrivateType _parser = null;

        [OneTimeSetUp]
        public void Init()
        {
            _parser = new PrivateType(typeof(CommandParser));
        }


        [Test]
        public void IsArgumentValueWithTwoEmptyParamsShouldReturnFalse()
        {
            Type[] parameterTypes       = { typeof(string), typeof(string) };
            object[] parameterValues    = {string.Empty, string.Empty};
            bool result                 = (bool)_parser.InvokeStatic("IsArgumentValue", parameterTypes, parameterValues);
            result.Should().BeFalse("Both of the parameters are empty");
        }

        [Test]
        public void IsArgumentValueWithInvalidParamShouldReturnFalse()
        {
            Type[] parameterTypes       = { typeof(string), typeof(string) };
            object[] parameterValues    = { "<j ", "test" };
            bool result                 = (bool)_parser.InvokeStatic("IsArgumentValue", parameterTypes, parameterValues);
            result.Should().BeFalse("InputSplit parameter does not have following bigger than character (>).");
        }


        [Test]
        public void IsArgumentValueWithValidParamsShouldReturnTrue()
        {
            Type[] parameterTypes       = { typeof(string), typeof(string) };
            object[] parameterValues    = { "<12345>", "test"};
            bool result = (bool)_parser.InvokeStatic("IsArgumentValue", parameterTypes, parameterValues);
            result.Should().BeTrue("Both parameters are valid.");
        }

        [Test]
        public void ParseInputDataWithGivenInputTextShouldReturnObjWithTwoArgsAndCorrectMethodName()
        {
            var cmdInput    = "Read File <test> SortByStartDate";
            var result      = CommandParser.ParseInputData(cmdInput, "TestClass");
            result.MethodName.ShouldBeEquivalentTo("Read");
            result.Arguments.Count.Should().Be(2);
        }

        [Test]
        public void ParseInputDataWithGivenInputTextShouldReturnObjWithThreeArgsAndCorrectClassName()
        {
            var cmdInput    = "Read File   <c:\\test\\text.txt> SortByStartDate Project <2>";
            var result      = CommandParser.ParseInputData(cmdInput, "TestClass");
            result.ClassName.ShouldBeEquivalentTo("TestClass");
            result.Arguments.Count.Should().Be(3);
        }


        [Test]
        public void ParseInputDataWithGivenInputTextShouldReturnCorrectArgValues()
        {
            var cmdInput    = "Read File <c:\\test\\test text.txt> SortByStartDate Project <2>";
            var result      = CommandParser.ParseInputData(cmdInput, "TestClass");
            result.Arguments.Count.Should().Be(3);
            result.Arguments["file"].ShouldBeEquivalentTo("c:\\test\\test text.txt");
            result.Arguments["sortbystartdate"].ShouldBeEquivalentTo(string.Empty);
            result.Arguments["project"].ShouldBeEquivalentTo("2");
        }
    }
}
