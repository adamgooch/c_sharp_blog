using System.Security.Cryptography;

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

        public byte[] GeneratePasswordDigest(string password, byte[] salt, int iterations)
        {
            var key = new Rfc2898DeriveBytes(password, salt, iterations);
            return key.GetBytes( DIGEST_BYTE_LENGTH );
        }
    }
}
