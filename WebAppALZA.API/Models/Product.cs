﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppALZA.API.Models
{
    //[Table("Products")]
    public class Product
    {
        [Key]        
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]                
        public string ImgPath { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public string Description{ get; set; }
        
    }
}
