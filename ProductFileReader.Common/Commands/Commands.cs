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
    public static class Commands
    {
        [ValueRequiredForParams("file", "project")]
        public static string Read(string file, int? project = null, bool sortbystartdate = false)
        {
            try
            {
                int noOfValueRows;
                var fileDataColumns = FileReader.ReadFileData(file, out noOfValueRows);
                var productData = FileDataHandler.DataToObjects<ProductData>(fileDataColumns, noOfValueRows);

                if (project.HasValue)
                    productData = FileDataHandler.FilterBy(productData, project.Value, pd => pd.Project);
                if (sortbystartdate)
                    productData = FileDataHandler.SortBy(productData, pd => pd.StartDate);

                return CreateProductDataOutput(productData, fileDataColumns);

            }
            catch (InputException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new FatalException(Constants.ErrorMessages.FatalError);
            }
        }

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

        private static StringBuilder WriteHeaderData(IEnumerable<FileDataColumn> inputCols, StringBuilder bob)
        {
            foreach (var inputCol in inputCols)
            {
                bob.AppendFormat("{0}\t", inputCol.HeaderTitle);
            }
            return bob;
        }

        private static StringBuilder WritePropertyValues(ProductData prodData, IEnumerable<FileDataColumn> inputCols,
            StringBuilder bob)
        {
            foreach (var inputCol in inputCols)
            {
                bob = WritePropertyValue(prodData, inputCol, bob);
            }
            return bob;
        }

        private static StringBuilder WritePropertyValue(ProductData prodData,  FileDataColumn inputCol, StringBuilder bob)
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
