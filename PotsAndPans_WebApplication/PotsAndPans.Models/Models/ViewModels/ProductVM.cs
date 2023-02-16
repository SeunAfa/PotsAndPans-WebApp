using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.Models.Models.ViewModels
{
    public class ProductVM
    {
        public  Product Product {get; set;}
        [ValidateNever]
        //Select List Items from Categorys
        public IEnumerable<SelectListItem> CategoryList { get; set;}    
      
    }
}
