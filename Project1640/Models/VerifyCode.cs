using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project1640.Models
{
    public class VerifyCode
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        public string Date { get; set; }
    }
}