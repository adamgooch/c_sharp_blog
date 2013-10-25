using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models.PageModels
{
    public class ResetPasswordPage
    {
        [Required( ErrorMessage = "Password is required" )]
        [StringLength( 64, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long." )]
        [DataType( DataType.Password )]
        public string Password { get; set; }

        [Compare( "Password", ErrorMessage = "Passwords do not match" )]
        [DataType( DataType.Password )]
        public string PasswordConfirmation { get; set; }
    }
}