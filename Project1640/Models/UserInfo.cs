using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project1640.Models
{
    public class UserInfo : IdentityUser
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string WorkingPlace { get; set; }

        [DataType(DataType.Custom)]
        public string DoB { get; set; }
        public List<Idea> listIdea { get; set; }
        public List<Comment> listReact { get; set; }
        public List<React> listComment { get; set; }

        public List<FileUpload> listFile { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
                                                   
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}