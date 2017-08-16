using ProductFileReader.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ProductFileReader.Common.Exceptions;

namespace ProductFileReader.Common.Utilities
{
    /// <summary>
    /// A handler class to parse input data to relevant object types and to filter and sort the data. 
    /// </summary>
    public static class FileDataHandler
    {

        public static IEnumerable<T> DataToObjects<T>(IEnumerable<FileDataColumn> dataCols, int noOfRows)
        {
            var result                  = new List<T>();
            var productDataProperties   = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var r = 0; r < noOfRows; r++)
            {
                var prodData = Activator.CreateInstance(typeof(T));
                for (var i = 0; i < productDataProperties.Count(); i++)
                {
                    var prop            = productDataProperties[i];
                    var propertyType    = prop.PropertyType;
                    var displayName     = prop.GetCustomAttribute<DisplayAttribute>().Name;
                    var dataCol         = dataCols.FirstOrDefault(dc => dc.HeaderTitle == displayName);
                    if(dataCols.Count() != productDataProperties.Length) throw new InputException(Constants.ErrorMessages.InvalidNumberOfColsInFile);
                    var valueAsString   = dataCol == null ? string.Empty : dataCol.Values[r];
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
                        throw new InputException(string.Format(ex.Message, r + 1));
                    }

                }
                result.Add((T)prodData);
            }
            return result;
        }

        
        /// <summary>
        /// Try to parse the string value into a given property type.
        /// </summary>
        /// <param name="propertyType">Property type</param>
        /// <param name="valueAsString">Input string value</param>
        /// <returns>Parsed object.</returns>
        private static object ParseStringValue(Type propertyType, string valueAsString)
        {
            //Change the number and datetime formats of the current thread. 
            var customCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            customCulture.NumberFormat.NumberGroupSeparator = "";
            customCulture.DateTimeFormat.TimeSeparator = ":";
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
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType.Name));
            }
            else if (propertyType == typeof(decimal?))
            {
                decimal decimalValue;
                if (decimal.TryParse(valueAsString, NumberStyles.AllowDecimalPoint, customCulture, out decimalValue))
                {
                    return decimalValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, typeof(decimal).Name));
            }
            else if (propertyType == typeof(bool))
            {
                bool boolValue;
                if (bool.TryParse(valueAsString, out boolValue))
                {
                    return boolValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType.Name));
            }
            else if (propertyType == typeof(DateTime))
            {
                DateTime dateTimeValue;
                if (DateTime.TryParseExact(valueAsString, Constants.Formats.DateTimeFormat, customCulture, DateTimeStyles.None, out dateTimeValue))
                {
                    return dateTimeValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType.Name));
            }
            else if (propertyType == typeof(ComplexityType))
            {
                ComplexityType complexityTypeValue;
                if (Enum.TryParse(valueAsString, out complexityTypeValue))
                {
                    return complexityTypeValue;
                }
                throw new Exception(string.Format(Constants.ErrorMessages.InvalidDataValue, valueAsString, propertyType.Name));
            }
            return null;
        }

        /// <summary>
        /// A generic method to filter data by a specific object property value.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="data">Data to be filtered.</param>
        /// <param name="filterValue">Filter value.</param>
        /// <param name="lambda">Filter property lambda expression.</param>
        /// <returns>Filtered data.</returns>
        public static IEnumerable<T> FilterBy<T>(IEnumerable<T> data, object filterValue, Expression<Func<T,object>> lambda )
        {
            var propInfo = GetPropertyInfo(lambda);
            return data.Where(di => propInfo.GetValue(di, null).Equals(filterValue));
        }


        /// <summary>
        /// A generic method to sort data by a specific object property.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="data">Data to be sorted.</param>
        /// <param name="lambda">Sort property lambda expression.</param>
        /// <param name="desc">Sort by descending.</param>
        /// <returns>Sorted data.</returns>
        public static IEnumerable<T> SortBy<T>(IEnumerable<T> data, Expression<Func<T, object>> lambda, bool desc = false)
        {
            var propInfo = GetPropertyInfo(lambda);
            return desc
                ? data.OrderByDescending(di => propInfo.GetValue(di, null))
                : data.OrderBy(di => propInfo.GetValue(di, null));
        }

        /// <summary>
        /// Get property information from lambda expression.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="lambda">Property information lambda expression.</param>
        /// <returns>Property information.</returns>
        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> lambda)
        {
            PropertyInfo propInfo;
            if (lambda.Body is MemberExpression)
            {
                propInfo = ((MemberExpression)lambda.Body).Member as PropertyInfo;
            }
            else {
                var op      = ((UnaryExpression)lambda.Body).Operand;
                propInfo    = ((MemberExpression)op).Member as PropertyInfo;
            }
            if (propInfo == null) throw new Exception();
            return propInfo;
        }


    }
}
