using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using PotsAndPans.Models.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.Models.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        //[DisplayName("Product Name")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        //Product Number
        //[Column("Product Number")]
        public string ProductNumber { get; set; }

        [Required]
        [Range(1,10000)]
        [DisplayName("List Price")]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Price for 1-50")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 51-100")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

        //Link Catgeory Model ID With ForeignKey
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

    }
}
