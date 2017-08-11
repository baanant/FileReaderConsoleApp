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
using ProductFileReader.Common.Utilities;

namespace ProductFileReader.Common.Commands
{
    public static class Commands
    {
        [ValueRequiredForParams("file","project")]
        public static string Read(string file, int? project = null, bool sortbystartdate = false)
        {
            try
            {
                int noOfValueRows;
                var fileDataColumns = FileReader.ReadFileData(file, out noOfValueRows);
                var productData     = FileDataHandler.DataToObjects<ProductData>(fileDataColumns, noOfValueRows);

                if (project.HasValue)
                    productData     = FileDataHandler.FilterBy(productData, project.Value, pd => pd.Project);
                //if(sortbystartdate)
                //    productData     = SortDataByStartDate(productData);

                return string.Empty;

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

        //private static string Create()

 


      
    

        
    }

 
}
