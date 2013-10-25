using Application.Users;
using Application.Utility;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Application
{
    public class Authenticator : IAuthenticate
    {
        public const string AuthenticationCookie = "ae23";
        public const byte CookieDelimeter = 0x7C;

        private IUserRepository userRepository;
        private IMailer mailer;

        public Authenticator( IUserRepository userRepository, IMailer mailer )
        {
            this.userRepository = userRepository;
            this.mailer = mailer;
        }

        public bool CreateUser( string email, string password, string passwordConfirmation )
        {
            if( !password.Equals( passwordConfirmation ) ) return false;
            var salt = ReplaceDelimeter( KeyGenerator.GenerateSalt() );
            var passwordDigest = KeyGenerator.GeneratePasswordDigest( password, salt );
            var verificationToken = Guid.NewGuid();
            var userCreated = userRepository.CreateUser( email, passwordDigest, salt, verificationToken );
            if( userCreated )
                return mailer.SendVerificationEmail( email, verificationToken );
            else
                return false;
        }

        public bool ActivateUser( Guid token )
        {
            return userRepository.ActivateUser( token );
        }

        public bool Authenticate( string email, string password )
        {
            var user = userRepository.GetUser( email );
            if( user == null ) return false;
            var passwordDigest = KeyGenerator.GeneratePasswordDigest( password, user.Salt );
            return passwordDigest.SequenceEqual( user.PasswordDigest ) && user.Active;
        }

        public HttpCookie GenerateAuthCookie( string email, bool rememberMe )
        {
            var user = userRepository.GetUser( email );
            var saltAsString = StringConversion.GetString( user.Salt );
            var value = String.Format( "{0}{1}{2}", user.Email, Convert.ToChar( CookieDelimeter ), saltAsString );
            var valueAsBytes = StringConversion.GetBytes( value );
            var encryptedValue = AesEncryption.EncryptBytes( valueAsBytes );
            var cookie = new HttpCookie( AuthenticationCookie )
            {
                Value = HttpServerUtility.UrlTokenEncode( encryptedValue ),
                HttpOnly = true
            };
            if( rememberMe ) cookie.Expires = DateTime.Now.AddDays( 7d );
            return cookie;
        }

        public User GetUser( HttpCookie authenticationCookie )
        {
            var cypherText = authenticationCookie.Value;
            var valueAsBytes = HttpServerUtility.UrlTokenDecode( cypherText );
            var value = AesEncryption.DecryptToBytes( valueAsBytes );
            var valueAsString = StringConversion.GetString( value );
            var valueParts = valueAsString.Split( new char[] { Convert.ToChar( CookieDelimeter ) } );
            var cookieEmail = valueParts[0];
            var cookieSalt = valueParts[1];
            var user = userRepository.GetUser( cookieEmail );
            var saltAsString = StringConversion.GetString( user.Salt );
            if( cookieSalt.Equals( saltAsString ) ) return user;
            return null;
        }

        public bool SendPasswordResetEmail( string email )
        {
            var resetToken = KeyGenerator.GenerateSalt();
            var success = userRepository.SetUserResetPasswordToken( email, resetToken );
            if( success )
            {
                var encryptedToken = AesEncryption.EncryptBytes( resetToken );
                var encryptedTokenAsString = HttpServerUtility.UrlTokenEncode( encryptedToken );
                success = mailer.SendPasswordResetEmail( email, encryptedTokenAsString );
            }
            return success;
        }

        public bool ChangePassword( string password, string passwordConfirmation )
        {
            return false;
        }

        public bool ResetPassword( string password, string passwordConfirmation, string token )
        {
            if( !password.Equals( passwordConfirmation ) ) return false;
            var salt = ReplaceDelimeter( KeyGenerator.GenerateSalt() );
            var passwordDigest = KeyGenerator.GeneratePasswordDigest( password, salt );
            var decodedToken = HttpServerUtility.UrlTokenDecode( token );
            var decryptedToken = AesEncryption.DecryptToBytes( decodedToken );
            if( userRepository.ResetPassword( passwordDigest, salt, decryptedToken ) ) return true;
            return false;
        }

        //made public to test
        public byte[] ReplaceDelimeter( byte[] array )
        {
            byte[] newArray = new byte[array.Length];
            for( int i = 0; i < array.Length; i++ )
            {
                if( array[i] == CookieDelimeter ) newArray[i] = 0xFF;
                else newArray[i] = array[i];
            }
            return newArray;
        }
    }
}
