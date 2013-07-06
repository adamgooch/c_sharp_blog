namespace Application
{
    public interface IAuthenticator
    {
        byte[] GenerateSalt();
        string GeneratePasswordDigest( string password, byte[] salt, int iterations );
    }
}
