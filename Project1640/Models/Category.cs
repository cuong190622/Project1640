using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project1640.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Need to input name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Idea> listIdea { get; set; }
    }
}