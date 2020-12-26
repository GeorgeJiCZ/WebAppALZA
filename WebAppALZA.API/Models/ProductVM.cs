using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace WebAppALZA.API.Models
{
    //[Table("Products")]
    public class ProductVM
    {          
        //[Required]        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]         
        //public Uri ImgPath { get; set; }        
        public string ImgPath { get; set; }

        [Required]        
        public decimal Price { get; set; }

        public string Description{ get; set; }
        
    }
}
