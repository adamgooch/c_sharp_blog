using System.ComponentModel.DataAnnotations;

namespace Web.Models.PageModels
{
    public class RegisterPage
    {
        [Required( ErrorMessage = "Email is required" )]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email" )]
        public string Email { get; set; }

        [Required( ErrorMessage = "Password is required" )]
        [StringLength( 64, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long." )]
        [DataType( DataType.Password )]
        public string Password { get; set; }

        [Compare( "Password", ErrorMessage = "Passwords do not match" )]
        [DataType( DataType.Password )]
        public string PasswordConfirmation { get; set; }
    }
}