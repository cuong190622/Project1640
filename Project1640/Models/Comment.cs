using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project1640.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Need to input content")]
        public string Content { get; set; }
        public bool Status { get; set; }
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public UserInfo User { get; set; }

        [Display(Name = "Idea")]
        public int IdeaId { get; set; }

        [ForeignKey("IdeaId")]
        public Idea UserIdea { get; set; }
    }
}