using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Commands
{
    public static class Commands
    {

        public static string Read(string file, int project = -1000 ,bool sortbystartdate = false)
        {
            try
            {
                int noOfValueRows = 0;
                var fileDataColumns = ReadFileData(file, out noOfValueRows);
                var productData = DataToObjects(fileDataColumns, noOfValueRows);
                return string.Empty;
            }
            catch (InputException ex)
            {
                throw ex;
            }
            catch (Exception e)
            {
                throw new FatalException(Constants.ErrorMessages.FatalError);
            }
        }

        private static List<ProductData> DataToObjects(List<FileDataColumn> dataCols, int noOfRows)
        {
            var result = new List<ProductData>();
            var productDataProperties = typeof (ProductData).GetProperties();
            for (var r = 0; r < noOfRows; r++)
            {
                var prodData = new ProductData();
                for (var i = 0; i < productDataProperties.Count(); i++)
                {
                    var prop = productDataProperties[i];
                    var propertyType = prop.PropertyType;
                    var displayName = prop.GetCustomAttribute<DisplayAttribute>().Name;
                    var dataCol = dataCols.FirstOrDefault(dc => dc.HeaderTitle == displayName);
                    var valueAsString = dataCol == null ? string.Empty : dataCol.Values[r];
                    var valueAsObject = ParseDataValue(propertyType, valueAsString);
                }

            }
            return null;
        }


        private static object ParseDataValue(Type propertyType, string valueAsString)
        {
            object result = null;
            if (propertyType == typeof(string))
            {
                result = valueAsString;
            }
            else if (propertyType == typeof(Int32))
            {
                int numberValue;
                if (int.TryParse(valueAsString, out numberValue))
                {
                    result = numberValue;
                }
                else
                {
                    throw new InputException("ADD");
                }
            }
            
            return result;
        }

        private static List<FileDataColumn> ReadFileData(string fileName, out int noOfRows)
        {
            noOfRows = 0;
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = string.Empty;
                var headerRow = true;
                var data = new List<FileDataColumn>();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
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
                            data[i].Values.Add(values[i].Contains("NULL") ? string.Empty : values[i]);
                        }
                    }
                }
                return data;
            }
        } 

    }
}
