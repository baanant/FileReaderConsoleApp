using System.Collections.Generic;

namespace ProductFileReader.Common.Entities
{
    /// <summary>
    /// Simple data column class to collect the data from .txt files.
    /// </summary>
    public class FileDataColumn
    {
        public FileDataColumn(string header)
        {
            HeaderTitle = header;
            Values      = new List<string>();
        }

        public string HeaderTitle { get; set; }

        public List<string> Values { get; set; }  
    }
}
