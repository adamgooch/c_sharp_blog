using System;
using System.Collections.Generic;

namespace Application.Users
{
    public interface IUserRepository
    {
        bool CreateUser( string email, byte[] passwordDigest, byte[] salt, Guid verificationToken );
        bool DeleteUser( Guid id );
        bool ActivateUser( Guid token );
        bool SetActive( Guid userId, bool active );
        bool AddRole( Guid userId, string role );
        bool RemoveRole( Guid userId, string role );
        bool SetUserResetPasswordToken( string email, byte[] resetToken );
        bool ResetPassword( byte[] passwordDigest, byte[] salt, byte[] token );
        User GetUser( string email );
        IEnumerable<User> GetAllUsers();
        IEnumerable<string> GetAllRoles();
    }
}
