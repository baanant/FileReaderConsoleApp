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
        private string      _properFilePath;
        private string      _invalidFilePath;
        private string      _invalidFilePath2;
        private PrivateType _fileReader;

        [OneTimeSetUp]
        public void Init()
        {
            const string fileName   = "inputFile1.txt";
            const string fileName2  = "inputFile2.txt";
            const string fileName3  = "inputFile3.txt";
            _properFilePath         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName);
            _invalidFilePath        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName2);
            _invalidFilePath2       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\", fileName3);
            _fileReader             = new PrivateType(typeof(FileReader));
        }


        [Test]
        public void ReadFileDataShouldReturnCorrectNumberOfRows()
        {
            int numberOfRows;
            var result = FileReader.ReadFileData(_properFilePath, out numberOfRows);
            result.Count().Should().Be(8, "Because the file has 8 rows.");
        }

        [Test]
        public void ReadFileDataShouldThrowInputExceptionDueToInvalidInputFileData()
        {
            int numberOfRows;
            Assert.Throws<InputException>(
                () => FileReader.ReadFileData(_invalidFilePath, out numberOfRows), "Because one row has wrong number of values.");
        }


        [Test]
        public void ReadFileDataShouldThrowInputExceptionDueToInvalidFileType()
        {
            int numberOfRows;
            var ex = Assert.Throws<InputException>(
                () => FileReader.ReadFileData(_invalidFilePath2, out numberOfRows), "Because the file extension is wrong.");
            ex.Message.ShouldBeEquivalentTo(Constants.ErrorMessages.InvalidFilePath);
        }

        [Test]
        public void IsFileValidShouldReturnTrue()
        {
            var result = (bool) _fileReader.InvokeStatic("IsFileValid", _properFilePath);
            result.Should().BeTrue("Because file path is valid.");
        }

        [Test]
        public void IsFileValidShouldReturnFalse()
        {
            var result = (bool)_fileReader.InvokeStatic("IsFileValid", _invalidFilePath2);
            result.Should().BeFalse("Because file path is invalid.");
        }

        [Test]
        public void HasSameNumberOfValuesShouldReturnTrue()
        {
            var dataColumns = new List<FileDataColumn>();
            var colOne      = new FileDataColumn("Project");
            colOne.Values.Add("1");
            colOne.Values.Add("2");
            colOne.Values.Add("2");
            dataColumns.Add(colOne);

            var colTwo      = new FileDataColumn("Complexity");
            colTwo.Values.Add("Simple");
            colTwo.Values.Add("Moderate");
            colTwo.Values.Add("Moderate");
            dataColumns.Add(colTwo);
            Type[] parameterTypes       = { typeof(IEnumerable<FileDataColumn>) };
            object[] parameterValues    = { dataColumns };
            var result                  = (bool) _fileReader.InvokeStatic("HasSameNumberOfValues", parameterTypes, parameterValues);
            result.Should().BeTrue("Because columns has the same number of values.");
        }


        [Test]
        public void HasSameNumberOfValuesShouldReturnFalse()
        {
            var dataColumns = new List<FileDataColumn>();
            var colOne      = new FileDataColumn("Project");
            colOne.Values.Add("1");
            colOne.Values.Add("2");
            colOne.Values.Add("2");
            dataColumns.Add(colOne);

            var colTwo      = new FileDataColumn("Complexity");
            colTwo.Values.Add("Simple");
            colTwo.Values.Add("Moderate");
            dataColumns.Add(colTwo);
            Type[] parameterTypes       = { typeof(IEnumerable<FileDataColumn>) };
            object[] parameterValues    = { dataColumns };
            var result                  = (bool)_fileReader.InvokeStatic("HasSameNumberOfValues", parameterTypes, parameterValues);
            result.Should().BeFalse("Because columns does not have the same number of values.");
        }

    }
}
