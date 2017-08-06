using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static List<FileDataColumn> ReadFileData(string fileName)
        {
            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(fileName))
            {
                // Read the stream to a string, and write the string to the console.
                string line = string.Empty;
                var headerRow = true;
                var data = new List<FileDataColumn>();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.StartsWith("#") || String.IsNullOrEmpty(line)) continue;
                    if (headerRow)
                    {
                        var headers = line.Split('\t').ToList();
                        headers.ForEach(h =>
                        {
                            if (!String.IsNullOrEmpty(h)) data.Add(new FileDataColumn(h));
                        });
                        headerRow = false;
                    }
                    else
                    {
                        var values = line.Split('\t').ToList();
                        for (var i = 0; i < values.Count(); i++)
                        {
                            if (values[i] == "NULL")
                                data[i].Values.Add(string.Empty);
                            else
                                data[i].Values.Add(values[i]);
                        }
                    }

                }

                return line;
            }
        } 

    }
}
