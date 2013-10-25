using Application.Users;
using System;
using System.Web;

namespace Application
{
    public interface IAuthenticate
    {
        bool CreateUser( string email, string password, string passwordConfirmation );
        bool Authenticate( string email, string password );
        HttpCookie GenerateAuthCookie( string email, bool rememberMe );
        User GetUser( HttpCookie authenticationCookie );
        bool SendPasswordResetEmail( string email );
        bool ChangePassword( string password, string passwordConfirmation );
        bool ResetPassword( string password, string passwordConfirmation, string token );
        bool ActivateUser( Guid token );
    }
}
