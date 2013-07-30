using System;
using System.Web;

namespace Application
{
    public interface IAuthenticator
    {
        byte[] GenerateSalt();
        byte[] GeneratePasswordDigest( string password, byte[] salt, int iterations );
        bool Authenticate( string password, byte[] salt, byte[] passwordDigest, int iterations );
        HttpCookie GenerateAuthenticationCookie( Guid id, byte[] salt );
        HttpCookie DecryptAuthenticationCookie( HttpCookie encryptedCookie );
        bool LoggedIn( Guid id, HttpCookie cookie );
    }
}
