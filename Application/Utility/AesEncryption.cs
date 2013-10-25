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
        private static readonly byte[] Key = { 151, 226, 32, 113, 4, 157, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 156, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private static readonly byte[] Vector = { 142, 64, 191, 167, 23, 3, 104, 119, 231, 121, 251, 112, 79, 43, 114, 201 };

        public static byte[] EncryptBytes( byte[] clearText )
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            ICryptoTransform encryptorTransform = rijndael.CreateEncryptor( Key, Vector );
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cs = new CryptoStream( memoryStream, encryptorTransform, CryptoStreamMode.Write );
            cs.Write( clearText, 0, clearText.Length );
            cs.FlushFinalBlock();

            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read( encrypted, 0, encrypted.Length );

            cs.Close();
            memoryStream.Close();
            return encrypted;
        }

        public static byte[] DecryptToBytes( byte[] EncryptedValue )
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            ICryptoTransform decryptorTransform = rijndael.CreateDecryptor( Key, Vector );

            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream( encryptedStream, decryptorTransform, CryptoStreamMode.Write );
            decryptStream.Write( EncryptedValue, 0, EncryptedValue.Length );
            decryptStream.FlushFinalBlock();

            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read( decryptedBytes, 0, decryptedBytes.Length );
            encryptedStream.Close();
            return decryptedBytes;
        }

        static public byte[] GenerateEncryptionKey()
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        static public byte[] GenerateEncryptionVector()
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }
    }
}
