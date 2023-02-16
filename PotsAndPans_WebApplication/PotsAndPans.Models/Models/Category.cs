using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PotsAndPans.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
      
        [Required]
        //[DisplayName("Product Category Name")]
        public string Name { get; set; }

        //[DisplayName("Display Order Number")]
        //[Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100 only!!")]
        //public int DisplayOrderNumber { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
