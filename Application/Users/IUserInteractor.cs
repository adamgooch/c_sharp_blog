using System;

namespace Application.Users
{
    public interface IUserInteractor
    {
        void CreateUser( string email, string password );
        void VerifyUser( Guid token );
        User GetUserByUsername( string username );
    }
}