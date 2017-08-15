using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using ProductFileReader.Common.Commands;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;
using ProductFileReader.Common.Utilities;

namespace ProductFileReader.UnitTests
{
    [TestFixture]
    public class CommandTests
    {
        private string _properFilePath;
        private string _invalidFilePath;
        private string _invalidFilePath2;

        [OneTimeSetUp]
        public void Init()
        {
            const string fileName   = "inputFile1.txt";
            const string fileName2  = "inputFile2.txt";
            const string fileName3  = "inputFile3.txt";
            _properFilePath         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName);
            _invalidFilePath        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName2);
            _invalidFilePath2       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName3);
        }

        [Test]
        public void ReadTestShouldNotThrowException()
        {
            Commands.Read(_properFilePath);
        }

        [Test]
        public void ReadTestShouldThrowExceptionBecauseOfInvalidFileContent()
        {
            Assert.Throws<InputException>(
                () => Commands.Read(_invalidFilePath), "Invalid file content.");
        }

        [Test]
        public void ReadTestShouldThrowExceptionBecauseOfInvalidFilePathTwo()
        {
            Assert.Throws<InputException>(
                () => Commands.Read(_invalidFilePath2), "Invalid file path.");
        }

    }
}
