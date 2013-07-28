using System;
using System.Collections.Generic;

namespace Application.Users
{
    public interface IUserRepository
    {
        void CreateUser( User user );
        IEnumerable<User> GetAllUsers();
        void SaveUser( User user );
        void DeleteById( Guid id );
    }
}
