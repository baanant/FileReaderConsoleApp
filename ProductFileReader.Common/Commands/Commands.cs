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
                var fileDataColumns = ReadFileData(file);
                var productData = DataToObjects(fileDataColumns);
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

        private static List<ProductData> DataToObjects(List<FileDataColumn> dataCols)
        {
            var productDataProperties = typeof (ProductData).GetProperties();
            foreach (var prop in productDataProperties)
            {
                var propertyType = prop.PropertyType;
                var displayName = prop.GetCustomAttribute<DisplayAttribute>().Name;

            }
            return null;
        }

        private static List<FileDataColumn> ReadFileData(string fileName)
        {
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
