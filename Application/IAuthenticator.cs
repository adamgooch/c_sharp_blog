using System;
using System.Web;
using Application.Users;

namespace Application
{
    public interface IAuthenticator
    {
        byte[] GenerateSalt();
        byte[] GeneratePasswordDigest( string password, byte[] salt, int iterations );
        void SendNewUserVerificationEmail( User user );
        bool Authenticate( string password, byte[] salt, byte[] passwordDigest, int iterations );
        HttpCookie GenerateAuthenticationCookie( Guid id, byte[] salt );
        HttpCookie DecryptAuthenticationCookie( HttpCookie encryptedCookie );
    }
}
