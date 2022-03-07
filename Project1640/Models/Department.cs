using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project1640.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; } //set the primary key for this class
        [Required(ErrorMessage = "Need to input name")]
        public string Name { get; set; }//property

        public string Description { get; set; }//property

        public List<UserInfo> listUser { get; set; }

    }
}