using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Utilities
{
    /// <summary>
    /// A reader class to split tab separated text files into data columns.
    /// </summary>
    public static class FileReader
    {
        public static List<FileDataColumn> ReadFileData(string fileName, out int noOfRows)
        {
            noOfRows = 0;
            var data = new List<FileDataColumn>();
            using (var sr = new StreamReader(fileName))
            {
                var headerRow = true;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line != null && (line.StartsWith("#") || string.IsNullOrEmpty(line))) continue;
                    var values = line.Split('\t').ToList();
                    if (headerRow)
                    {
                        values.ForEach(h =>
                        {
                            if (!string.IsNullOrEmpty(h)) data.Add(new FileDataColumn(h));
                        });
                        headerRow = false;
                    }
                    else
                    {
                        noOfRows++;
                        for (var i = 0; i < values.Count(); i++)
                        {
                            data[i].Values.Add(values[i]);
                        }
                    }
                }
            }

            //ToDo: Get more descriptive error message here, with the row number.
            if (!data.HasSameNumberOfValues()) throw new InputException(Constants.ErrorMessages.InvalidInputFileData);
            return data;
        }


        /// <summary>
        /// A method to check if all the columns of the file has the same number of values.
        /// </summary>
        /// <param name="data">Read file column data.</param>
        /// <returns>True if all the columns has the the same number of values, otherwise false.</returns>
        private static bool HasSameNumberOfValues(this IEnumerable<FileDataColumn> data)
        {
            if (data == null || data.First().Values == null) return false;
            var firstCount = data.First().Values.Count();
            return data.All(dc => dc.Values.Count() == firstCount);
        }
    }
}
