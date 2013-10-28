using Application.Users;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Validations
{
    public class UniqueEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            var userRepository = new SqlServerUserRepository();
            var userInteractor = new UserInteractor( userRepository );
            var user = userInteractor.GetUser( value.ToString() );
            if( user != null )
            {
                return new ValidationResult( this.FormatErrorMessage( validationContext.DisplayName ) );
            }
            return null;
        }
    }
}