using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProductFileReader.Common.Entities
{
    public class ProductData
    {
        [Display(Name = "Project")]
        public int Project { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Responsible")]
        public string Responsible { get; set; }

        [Display(Name = "SavingsAmount")]
        public decimal SavingsAmount { get; set; }

        [Display(Name ="Currency")]
        public string Currency { get; set; }

        [Display(Name = "Complexity")]
        public ComplexityType Complexity { get; set; }
    }
}
