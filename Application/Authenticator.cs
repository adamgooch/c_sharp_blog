using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Application.Users;

namespace Application
{
    public class Authenticator : IAuthenticator
    {
        private const int SALT_BYTE_LENGTH = 16;
        private const int DIGEST_BYTE_LENGTH = 32;

        public byte[] GenerateSalt()
        {
            var salt = new byte[SALT_BYTE_LENGTH];
            using( var rngCsp = new RNGCryptoServiceProvider() )
            {
                rngCsp.GetBytes( salt );
            }
            return salt;
        }

        public byte[] GeneratePasswordDigest( string password, byte[] salt, int iterations )
        {
            var key = new Rfc2898DeriveBytes( password, salt, iterations );
            return key.GetBytes( DIGEST_BYTE_LENGTH );
        }

        public void SendNewUserVerificationEmail( User user )
        {
            var fromAddress = new MailAddress( "adamgooch@outlook.com", "Adam Gooch" );
            var toAddress = new MailAddress( user.Email, user.Email );
            const string fromPassword = "@nw7jrUHr7Nf*2CV";
            const string subject = "AdamGooch.me New User Verification";
            string body = String.Format( "Follow this link to complete your registration: http://localhost:50508/verify_user/{0}", user.VerifiedToken );
            var smtp = new SmtpClient
                {
                    Host = "smtp.live.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential( fromAddress.Address, fromPassword )
                };
            using( var message = new MailMessage( fromAddress, toAddress )
            {
                Subject = subject,
                Body = body
            } )
            {
                smtp.Send( message );
            }
        }
    }
}
