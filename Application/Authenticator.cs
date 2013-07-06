namespace Application
{
    class Authenticator : IAuthenticator
    {
        public byte[] GenerateSalt()
        {
            return new byte[16];
        }

        public string GeneratePasswordDigest(string password, byte[] salt, int iterations)
        {
            return "whatever";
        }
    }
}
