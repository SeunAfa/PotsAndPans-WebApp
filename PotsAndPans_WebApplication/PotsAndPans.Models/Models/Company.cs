using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.Models.Models
{
    public class Company
    {
        public int Id { get; set; } 

        [Required]
        public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Borough { get; set; }
        public string? PostCode { get; set; }
    }
}
