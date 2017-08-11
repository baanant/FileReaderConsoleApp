using System;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Savings amount")]
        public decimal? SavingsAmount { get; set; }

        [Display(Name ="Currency")]
        public string Currency { get; set; }

        [Display(Name = "Complexity")]
        public ComplexityType Complexity { get; set; }


    }
}
