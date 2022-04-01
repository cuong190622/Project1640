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
        [Required(ErrorMessage = "Need to input Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Need to input content")]
        public string Content { get; set; }
        public bool Status { get; set; }
        public int Rank { get; set; }
        public int Views { get; set; }
        [DataType(DataType.Date)]
        public string Date { get; set; }

        public List<React> listReact { get; set; }

        public List<Comment> listComment { get; set; }

        public List<FileUpload> listFile { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }


        [Display(Name = "Category")]
        public int CategoryId { get; set; }


    }
}