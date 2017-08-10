using System.Collections.Generic;

namespace ProductFileReader.Common.Entities
{
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
