using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project1640.Models
{
    public class Idea
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Need to input content")]
        public string Content { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Custom)]
        public string Date { get; set; }
        public List<Category> listCategory { get; set; }
        public Idea()
        {
            listCategory = new List<Category>();//Code first Many to Many relationship

        }
        public List<React> listReact { get; set; }

        public List<Comment> listComment { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public  UserInfo User { get; set; }

    }
}