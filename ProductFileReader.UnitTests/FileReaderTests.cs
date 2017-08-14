using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;
using ProductFileReader.Common.Utilities;
using Assert = NUnit.Framework.Assert;

namespace ProductFileReader.UnitTests
{
    [TestFixture]

    public class FileReaderTests
    {
        string      _properFilesPath;
        string      _invalidFilesPath;
        PrivateType _fileReader;

        [OneTimeSetUp]
        public void Init()
        {
            string fileName     = "inputFile1.txt";
            string fileName2    = "inputFile2.txt";
            _properFilesPath    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName);
            _invalidFilesPath   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName2);
            _fileReader         = new PrivateType(typeof(FileReader));
        }


        [Test]
        public void ReadFileDataShouldReturnCorrectNumberOfRows()
        {
            object[] parameterValues    = { _properFilesPath, null};
            var result                  = (List<FileDataColumn>)_fileReader.InvokeStatic("ReadFileData", parameterValues);
            result.Count().Should().Be(8, "Because the file has 8 rows.");
        }

        [Test]
        public void ReadFileDataShouldThrowInputExceptionDueToInvalidInputFileData()
        {
            object[] parameterValues = { _invalidFilesPath, null };
            Assert.Throws<InputException>(
                () => _fileReader.InvokeStatic("ReadFileData", parameterValues), "Because one row has wrong number of values.");
        }


    }
}
