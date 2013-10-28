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
        
        private IUserRepository userRepository;
        private IMailer mailer;
        private AesEncryption encryptor;

        public Authenticator( IUserRepository userRepository, IMailer mailer )
        {
            this.userRepository = userRepository;
            this.mailer = mailer;
            encryptor = new AesEncryption();
        }

        public bool CreateUser( string email, string password, string passwordConfirmation )
        {
            if( !password.Equals( passwordConfirmation ) ) return false;
            var salt = PBKDF2Helper.GenerateSalt();
            var passwordDigest = PBKDF2Helper.GeneratePasswordDigest( password, salt );
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
            var passwordDigest = PBKDF2Helper.GeneratePasswordDigest( password, user.Salt );
            return passwordDigest.SequenceEqual( user.PasswordDigest ) && user.Active;
        }

        public HttpCookie GenerateAuthCookie( string email, bool rememberMe )
        {
            var user = userRepository.GetUser( email );
            var value = System.Text.Encoding.Default.GetString( user.Salt ) + user.Email;
            var cookie = new HttpCookie( AuthenticationCookie )
            {
                Value = Convert.ToBase64String( encryptor.Encrypt( value ) ),
                //Secure = true,
                HttpOnly = true
            };
            if( rememberMe ) cookie.Expires = DateTime.Now.AddDays( 7d );
            return cookie;
        }

        public User GetUser( HttpCookie authenticationCookie )
        {
            var decryptedValue = encryptor.Decrypt( Convert.FromBase64String( authenticationCookie.Value ) );
            var salt = decryptedValue.Substring( 0, PBKDF2Helper.SaltByteLength );
            var saltAsBytes = System.Text.Encoding.Default.GetBytes( salt );
            var email = decryptedValue.Substring( PBKDF2Helper.SaltByteLength );
            var user = userRepository.GetUser( email );
            if( user.Salt.SequenceEqual( saltAsBytes ) ) return user;
            return null;
        }

        public bool SendPasswordResetEmail( string email )
        {
            var resetToken = PBKDF2Helper.GenerateSalt();
            var success = userRepository.SetUserResetPasswordToken( email, resetToken );
            if( success )
            {
                var tokenAsString = System.Text.Encoding.Default.GetString( resetToken );
                var encryptedToken = encryptor.Encrypt( tokenAsString );
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
            var salt = PBKDF2Helper.GenerateSalt();
            var passwordDigest = PBKDF2Helper.GeneratePasswordDigest( password, salt );
            var decodedToken = HttpServerUtility.UrlTokenDecode( token );
            var decryptedToken = encryptor.Decrypt( decodedToken );
            var decryptedTokenAsBytes = System.Text.Encoding.Default.GetBytes( decryptedToken );
            if( userRepository.ResetPassword( passwordDigest, salt, decryptedTokenAsBytes ) ) return true;
            return false;
        }
    }
}
