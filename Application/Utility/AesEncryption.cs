using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utility
{
    class AesEncryption
    {
        private readonly string keyFile;
        private readonly string ivFile;
        private readonly string rootDirectory = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "keys";

        public AesEncryption()
        {
            keyFile = String.Format( "{0}\\key", rootDirectory );
            ivFile = String.Format( "{0}\\iv", rootDirectory );
        }

        public byte[] Encrypt( string plainText )
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

        public string Decrypt( byte[] cipherText )
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
