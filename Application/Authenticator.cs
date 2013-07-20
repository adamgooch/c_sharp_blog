using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using Application.Users;

namespace Application
{
    public class Authenticator : IAuthenticator
    {
        private const int SALT_BYTE_LENGTH = 16;
        private const int DIGEST_BYTE_LENGTH = 32;
        private const string AUTHENTICATION_COOKIE = "ae23";

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
            var fromAddress = new MailAddress( ConfigurationManager.AppSettings["EmailFromAddress"], "Adam Gooch" );
            var toAddress = new MailAddress( user.Email, user.Email );
            var fromPassword = ConfigurationManager.AppSettings["EmailPassword"];
            const string subject = "AdamGooch.me New User Verification";
            var body =
                String.Format( "Follow this link to complete your registration: http://localhost:50508/verify_user/{0}",
                              user.VerifiedToken );
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

        public bool Authenticate( string password, byte[] salt, byte[] passwordDigest, int iterations )
        {
            var testDigest = GeneratePasswordDigest( password, salt, iterations );
            return testDigest.SequenceEqual( passwordDigest );
        }

        public HttpCookie GenerateAuthenticationCookie( Guid id, byte[] salt, HttpSessionStateBase session )
        {
            var cookie = new HttpCookie( AUTHENTICATION_COOKIE );
            var encryptedId = Encrypt( id.ToString() );
            var encryptedSalt = Encrypt( System.Text.Encoding.Default.GetString( salt ) );
            cookie.Values.Add( "Id", System.Text.Encoding.Default.GetString( encryptedId ) );
            cookie.Values.Add( "Salt", System.Text.Encoding.Default.GetString( encryptedSalt ) );
            cookie.Values.Add( "Session", session.SessionID );
            return cookie;
        }

        public HttpCookie DecryptAuthenticationCookie( HttpCookie cookie )
        {
            var decryptedId = Decrypt( System.Text.Encoding.Default.GetBytes( cookie.Values["Id"] ) );
            var decryptedSalt = Decrypt( System.Text.Encoding.Default.GetBytes( cookie.Values["Salt"] ) );
            cookie.Values["Id"] = decryptedId;
            cookie.Values["Salt"] = decryptedSalt;
            return cookie;
        }

        private static byte[] Encrypt( string plainText )
        {
            byte[] encrypted;
            using( var aesAlg = new AesCryptoServiceProvider() )
            {
                var encryptor = aesAlg.CreateEncryptor( GetKey(), GetIV() );
                aesAlg.Padding = PaddingMode.PKCS7;

                using( var msEncrypt = new MemoryStream() )
                {
                    using( var csEncrypt = new CryptoStream( msEncrypt, encryptor, CryptoStreamMode.Write ) )
                    {
                        using( var swEncrypt = new StreamWriter( csEncrypt ) )
                        {
                            swEncrypt.Write( plainText );
                        }
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        private static string Decrypt( byte[] cipherText )
        {
            string plaintext = null;

            using( var aesAlg = Aes.Create() )
            {
                aesAlg.Key = GetKey();
                aesAlg.IV = GetIV();
                aesAlg.Padding = PaddingMode.PKCS7;

                var decryptor = aesAlg.CreateDecryptor( aesAlg.Key, aesAlg.IV );

                using( var msDecrypt = new MemoryStream( cipherText ) )
                {
                    using( var csDecrypt = new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read ) )
                    {
                        using( var srDecrypt = new StreamReader( csDecrypt ) )
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }

        private static byte[] GetKey()
        {
            var key = File.ReadAllBytes( ConfigurationManager.AppSettings["AESKey"] );
            return key;
        }

        private static byte[] GetIV()
        {
            var iv = File.ReadAllBytes( ConfigurationManager.AppSettings["AESIV"] );
            return iv;
        }

        private static void GenerateAESKey( AesCryptoServiceProvider aesAlg )
        {
            File.WriteAllBytes(ConfigurationManager.AppSettings["AESIV"], aesAlg.IV );
            File.WriteAllBytes(ConfigurationManager.AppSettings["AESKey"], aesAlg.Key);
        }
    }
}
