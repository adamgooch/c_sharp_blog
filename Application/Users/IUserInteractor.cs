using System;
using System.Collections.Generic;
using System.Web;

namespace Application.Users
{
    public interface IUserInteractor
    {
        void CreateUser( string email, string password );
        void VerifyUser( Guid token );
        User GetUserByUsername( string username );
        User GetUserById( Guid id );
        User GetUserByCookie( HttpCookie cookie );
        IEnumerable<User> GetAllUsers();
    }
}