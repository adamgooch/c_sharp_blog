namespace Application.Users
{
    public interface IUserInteractor
    {
        void CreateUser( string email, string password );
    }
}