using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppALZA.API.Models
{
    //[Table("Products")]
    public class Product
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        //[Column(TypeName = "string")]
        //public Uri ImgUri { get; set; }
        public string ImgUri { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public string Description{ get; set; }
        
    }
}
