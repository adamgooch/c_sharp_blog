using System;
using System.Collections.Generic;
using System.Web;

namespace Application.Users
{
    public interface IUserInteractor
    {
        bool DeleteUser( Guid id );
        bool SetActive( Guid userId, bool active );
        bool AddRole( Guid userId, string role );
        bool RemoveRole( Guid userId, string role );
        IEnumerable<User> GetAllUsers();
        IEnumerable<string> GetAllRoles();
        User GetUser( string email );
        bool UsernameExists( string email );
    }
}