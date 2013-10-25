using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utility
{
    public class KeyGenerator
    {
        private const int SaltByteLength = 64;
        private const int IterationCount = 3000;
        private const int DerivedKeyLength = 256;

        private static HMACSHA512 hmacsha512Obj;

        public static byte[] GenerateSalt()
        {
            var salt = new byte[SaltByteLength];
            using( var rngCsp = new RNGCryptoServiceProvider() )
            {
                rngCsp.GetBytes( salt );
            }
            return salt;
        }

        public static byte[] GeneratePasswordDigest( string password, byte[] salt )
        {
            return GetDerivedKeyBytes_PBKDF2_HMACSHA512( password, salt );
        }

        /*
         * This implementation is borrowed from
         * http://stackoverflow.com/questions/18648084/rfc2898-pbkdf2-with-sha256-as-digest-in-c-sharp
         * because I want to use SHA512 instead of Microsoft's implementation of SHA1
         * */
        private static Byte[] GetDerivedKeyBytes_PBKDF2_HMACSHA512( string password, byte[] salt )
        {
            byte[] passwordAsBytes = StringConversion.GetBytes( password );
            hmacsha512Obj = new HMACSHA512( passwordAsBytes );
            Int32 hLen = hmacsha512Obj.HashSize / 8;
            Double l = Math.Ceiling( (Double)DerivedKeyLength / hLen );
            Byte[] finalBlock = new Byte[0];
            for( Int32 i = 1; i <= l; i++ )
            {
                finalBlock = pMergeByteArrays( finalBlock, mainFunction( passwordAsBytes, salt, IterationCount, i ) );
            }

            return finalBlock.Take( DerivedKeyLength ).ToArray();
        }

        private static Byte[] mainFunction( Byte[] P, Byte[] S, Int32 c, Int32 i )
        {
            Byte[] Si = pMergeByteArrays( S, INT( i ) );
            Byte[] temp = PRF( P, Si );
            Byte[] U_c = temp;
            for( Int32 C = 1; C < c; C++ )
            {
                temp = PRF( P, temp );

                for( Int32 j = 0; j < temp.Length; j++ )
                {
                    U_c[j] ^= temp[j];
                }
            }
            return U_c;
        }

        private static Byte[] INT( Int32 i )
        {
            Byte[] I = BitConverter.GetBytes( i );
            if( BitConverter.IsLittleEndian )
            {
                Array.Reverse( I );
            }
            return I;
        }

        private static Byte[] PRF( Byte[] P, Byte[] S )
        {
            return hmacsha512Obj.ComputeHash( pMergeByteArrays( P, S ) );
        }

        private static Byte[] pMergeByteArrays( Byte[] source1, Byte[] source2 )
        {
            Byte[] buffer = new Byte[source1.Length + source2.Length];
            System.Buffer.BlockCopy( source1, 0, buffer, 0, source1.Length );
            System.Buffer.BlockCopy( source2, 0, buffer, source1.Length, source2.Length );
            return buffer;
        }
    }
}
