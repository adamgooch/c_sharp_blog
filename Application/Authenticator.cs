using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;

namespace Application
{
    public class Authenticator : IAuthenticator
    {
        public const string AuthenticationCookie = "ae23";
        private const int SaltByteLength = 16;
        private const int DigestByteLength = 32;
        private readonly string rootDirectory = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "keys";
        private readonly string keyFile;
        private readonly string ivFile;

        public Authenticator()
        {
            keyFile = String.Format( "{0}\\key", rootDirectory );
            ivFile = String.Format( "{0}\\iv", rootDirectory );
        }

        public byte[] GenerateSalt()
        {
            var salt = new byte[SaltByteLength];
            using( var rngCsp = new RNGCryptoServiceProvider() )
            {
                rngCsp.GetBytes( salt );
            }
            return salt;
        }

        public byte[] GeneratePasswordDigest( string password, byte[] salt, int iterations )
        {
            var key = new Rfc2898DeriveBytes( password, salt, iterations );
            return key.GetBytes( DigestByteLength );
        }

        public bool Authenticate( string password, byte[] salt, byte[] passwordDigest, int iterations )
        {
            var testDigest = GeneratePasswordDigest( password, salt, iterations );
            return testDigest.SequenceEqual( passwordDigest );
        }

        public HttpCookie GenerateAuthenticationCookie( Guid id, byte[] salt )
        {
            var value = System.Text.Encoding.Default.GetString( salt ) + id.ToString();
            var cookie = new HttpCookie( AuthenticationCookie )
                {
                    Value = Convert.ToBase64String( Encrypt( value ) ),
                    HttpOnly = true
                };
            return cookie;
        }

        public Guid GetId( HttpCookie encryptedCookie )
        {
            var decryptedCookie = DecryptAuthenticationCookie( encryptedCookie );
            return new Guid( decryptedCookie.Values["Id"] );
        }

        public byte[] GetSalt( HttpCookie encryptedCookie )
        {
            var decryptedCookie = DecryptAuthenticationCookie( encryptedCookie );
            return System.Text.Encoding.Default.GetBytes( decryptedCookie.Values["Salt"] );
        }

        public HttpCookie DecryptAuthenticationCookie( HttpCookie cookie )
        {
            cookie.Value = Decrypt( Convert.FromBase64String( cookie.Value ) );
            var salt = cookie.Value.Substring( 0, SaltByteLength );
            var id = cookie.Value.Substring( SaltByteLength );
            cookie.Values["Salt"] = salt;
            cookie.Values["Id"] = id;
            return cookie;
        }

        public bool LoggedIn( Guid id, HttpCookie decryptedCookie )
        {
            //var decryptedCookie = DecryptAuthenticationCookie( encryptedCookie );
            var cookieId = decryptedCookie.Values["Id"];
            return id == new Guid( cookieId );
        }

        private byte[] Encrypt( string plainText )
        {
            byte[] encrypted;
            using( var aesAlg = new AesCryptoServiceProvider() )
            {
                GenerateAesKeyIfNoneExists( aesAlg );
                var encryptor = aesAlg.CreateEncryptor( GetKey(), GetIv() );
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

        private string Decrypt( byte[] cipherText )
        {
            string plaintext = null;
            using( var aesAlg = Aes.Create() )
            {
                aesAlg.Key = GetKey();
                aesAlg.IV = GetIv();
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

        private byte[] GetKey()
        {
            var key = File.ReadAllBytes( keyFile );
            return key;
        }

        private byte[] GetIv()
        {
            var iv = File.ReadAllBytes( ivFile );
            return iv;
        }

        private void GenerateAesKeyIfNoneExists( AesCryptoServiceProvider aesAlg )
        {
            if( !Directory.Exists( rootDirectory ) ) Directory.CreateDirectory( rootDirectory );
            if( File.Exists( keyFile ) ) return;
            File.WriteAllBytes( ivFile, aesAlg.IV );
            File.WriteAllBytes( keyFile, aesAlg.Key );
        }
    }
}
