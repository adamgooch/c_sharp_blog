using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models.PageModels
{
    public class LoginPage
    {
        [Required]
        [Display( Name = "Username" )]
        public string UserName { get; set; }

        [Required]
        [DataType( DataType.Password )]
        [Display( Name = "Password" )]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}