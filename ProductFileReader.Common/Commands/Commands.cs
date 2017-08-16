using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;
using ProductFileReader.Common.CustomAttributes;
using ProductFileReader.Common.Utilities;

namespace ProductFileReader.Common.Commands
{
    /// <summary>
    /// A command class to be used in the console application.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// A command to read tab-separated UTF-8 text file, sort it and filter it.
        /// </summary>
        /// <param name="file">File path.</param>
        /// <param name="project">Project filter parameter.</param>
        /// <param name="sortbystartdate">Sort by date parameter.</param>
        /// <returns>String presentation as the output.</returns>
        [ValueRequiredForParams("file", "project")]
        public static string Read(string file, int? project = null, bool sortbystartdate = false)
        {
            try
            {
                int noOfValueRows;
                //Read the .txt file into data columns.
                var fileDataColumns = FileReader.ReadFileData(file, out noOfValueRows);
                //Create objects out of input data.
                var productData     = FileDataHandler.DataToObjects<ProductData>(fileDataColumns, noOfValueRows);

                //Filter data.
                if (project.HasValue)
                    productData     = FileDataHandler.FilterBy(productData, project.Value, pd => pd.Project);

                //Sort data.
                if (sortbystartdate)
                    productData     = FileDataHandler.SortBy(productData, pd => pd.StartDate, false);

                return CreateProductDataOutput(productData, fileDataColumns);

            }
            catch (InputException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// A method to write the output data from ProductData objects.
        /// </summary>
        /// <param name="productData"></param>
        /// <param name="inputColumns"></param>
        /// <returns></returns>
        private static string CreateProductDataOutput(IEnumerable<ProductData> productData, IEnumerable<FileDataColumn> inputColumns)
        {
            var bob         = new StringBuilder();
            var prodData    = productData.ToList();
            var inputCols   = inputColumns.ToList();
            bob             = WriteHeaderData(inputCols, bob);
            
            for (var i = 0; i < prodData.ToList().Count(); i++)
            {
                bob.Append(Environment.NewLine);
                var currentProd = prodData[i];
                bob             = WritePropertyValues(currentProd, inputCols, bob);
            }
            return bob.ToString();
        }

        /// <summary>
        /// Writes the header row by utilizing the same headers as in the input.
        /// </summary>
        /// <param name="inputCols">Input data columns.</param>
        /// <param name="bob">Builder to be returned.</param>
        /// <returns>StringBuilder object containing tab-separated header text row.</returns>
        private static StringBuilder WriteHeaderData(IEnumerable<FileDataColumn> inputCols, StringBuilder bob)
        {
            foreach (var inputCol in inputCols)
            {
                bob.AppendFormat("{0}\t", inputCol.HeaderTitle);
            }
            return bob;
        }

        /// <summary>
        /// Writes property values to a given string builder.
        /// </summary>
        /// <param name="prodData">Parsed product data object.</param>
        /// <param name="inputCols">Input data in columns.</param>
        /// <param name="bob">StringBuilder to be modified.</param>
        /// <returns>Modified StringBuilder object.</returns>
        private static StringBuilder WritePropertyValues(ProductData prodData, IEnumerable<FileDataColumn> inputCols,
            StringBuilder bob)
        {
            foreach (var inputCol in inputCols)
            {
                bob = WritePropertyValue(prodData, inputCol, bob);
            }
            return bob;
        }

        /// <summary>
        /// Write property value to the StringBuilder. Separate by tabs.
        /// </summary>
        /// <param name="prodData">Product data object.</param>
        /// <param name="inputCol">Input data column.</param>
        /// <param name="bob">StringBuilder to be modified.</param>
        /// <returns>Modified StringBuilder object.</returns>
        private static StringBuilder WritePropertyValue(ProductData prodData, FileDataColumn inputCol, StringBuilder bob)
        {
            var properties = prodData.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<DisplayAttribute>().Name != inputCol.HeaderTitle) continue;
                var value = prop.GetValue(prodData);
                value = value == null ? string.Empty : GetValidStringPresentation(value);
                bob.AppendFormat("{0}\t", value);
            }
            return bob;
        }

        /// <summary>
        /// Get the proper string presentation for decimal and datetime objects.
        /// </summary>
        /// <param name="value">Value as object.</param>
        /// <returns>Required string presentation.</returns>
        private static string GetValidStringPresentation(object value)
        {
            if (value is decimal)
            {
                return ((decimal)value).ToString(Constants.Formats.DecimalFormat);
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToString(Constants.Formats.DateTimeFormat);
            }
            return value.ToString();
        }







    }


}
