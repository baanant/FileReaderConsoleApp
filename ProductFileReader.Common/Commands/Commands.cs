using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ProductFileReader.Common.Entities;
using ProductFileReader.Common.Exceptions;
using ProductFileReader.Common.CustomAttributes;

namespace ProductFileReader.Common.Commands
{
    public static class Commands
    {
        [ValueRequiredForParams("file","project")]
        public static string Read(string file, int? project = null, bool sortbystartdate = false)
        {
            try
            {
                int noOfValueRows = 0;
                var fileDataColumns = ReadFileData(file, out noOfValueRows);
                var productData = DataToObjects(fileDataColumns, noOfValueRows);
                if(project.HasValue) productData = FilterByProject(productData, project.Value);
                if(sortbystartdate) productData = SortDataByStartDate(productData);
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


        //ToDo: Make more generic.
        private static IEnumerable<ProductData> FilterByProject(IEnumerable<ProductData> data, int projectId)
        {
            return data.Where(di => di.Project == projectId);
        } 

        //ToDo: Make more generic.
        private static IEnumerable<ProductData> SortDataByStartDate(IEnumerable<ProductData> data)
        {
            return data.OrderByDescending(di => di.StartDate);
        }

        private static IEnumerable<ProductData> DataToObjects(List<FileDataColumn> dataCols, int noOfRows)
        {
            var result = new List<ProductData>();
            var productDataProperties = typeof (ProductData).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
                    try
                    {
                        var valueAsObject = 
                            (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) || (propertyType == typeof(string))) 
                            && valueAsString == Constants.InputData.NullValue 
                            ? null : ParseStringValue(propertyType, valueAsString);
                        if (prop.CanWrite)
                        {
                            prop.SetValue(prodData, valueAsObject, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InputException(string.Format(ex.Message, r+1));
                    }
                    
                }
                result.Add(prodData);
            }
            return result;
        }


        private static object ParseStringValue(Type propertyType, string valueAsString)
        {
            var customCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            customCulture.NumberFormat.NumberGroupSeparator = "";
            Thread.CurrentThread.CurrentCulture = customCulture;
            if (propertyType == typeof(string))
            {
                return valueAsString;
            }
            else if (propertyType == typeof(int))
            {
                int numericValue;
                if (int.TryParse(valueAsString, out numericValue))
                {
                    return numericValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType));
            }
            else if (propertyType == typeof (decimal?)) 
            {
                decimal decimalValue;
                if (decimal.TryParse(valueAsString, NumberStyles.AllowDecimalPoint, customCulture, out decimalValue))
                {
                    return decimalValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType));
            }
            else if (propertyType == typeof (bool))
            {
                bool boolValue;
                if (bool.TryParse(valueAsString, out boolValue))
                {
                    return boolValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType));
            }
            else if (propertyType == typeof (DateTime))
            {
                DateTime dateTimeValue;
                if (DateTime.TryParseExact(valueAsString, Constants.Formats.DateTimeFormat, customCulture, DateTimeStyles.None, out dateTimeValue))
                {
                    return dateTimeValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType));
            }
            else if (propertyType == typeof (ComplexityType))
            {
                ComplexityType complexityTypeValue;
                if (Enum.TryParse(valueAsString, out complexityTypeValue))
                {
                    return complexityTypeValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType));
            }
            return null;
        }

        private static List<FileDataColumn> ReadFileData(string fileName, out int noOfRows)
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
            if(!data.HasSameNumberOfValues()) throw new InputException(Constants.ErrorMessages.InvalidInputFileData);
    
            return data;
        }


        
        private static bool HasSameNumberOfValues(this IEnumerable<FileDataColumn> data)
        {
            if (data == null || data.First().Values == null) return false;
            var firstCount = data.First().Values.Count();
            return data.All(dc => dc.Values.Count() == firstCount);
        } 

        
    }

 
}
