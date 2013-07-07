namespace Application
{
    public interface IAuthenticator
    {
        byte[] GenerateSalt();
        byte[] GeneratePasswordDigest( string password, byte[] salt, int iterations );
    }
}
