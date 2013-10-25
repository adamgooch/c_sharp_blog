using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Users
{
    public class UserInteractor : IUserInteractor
    {
        IUserRepository userRepository;

        public UserInteractor( IUserRepository userRepository )
        {
            this.userRepository = userRepository;
        }

        public bool DeleteUser( Guid id )
        {
            return userRepository.DeleteUser( id );
        }

        public bool SetActive( Guid userId, bool active )
        {
            return userRepository.SetActive( userId, active );
        }

        public bool AddRole( Guid userId, string role )
        {
            return userRepository.AddRole( userId, role );
        }

        public bool RemoveRole( Guid userId, string role )
        {
            return userRepository.RemoveRole( userId, role );
        }

        public IEnumerable<User> GetAllUsers()
        {
            return userRepository.GetAllUsers();
        }

        public IEnumerable<string> GetAllRoles()
        {
            return userRepository.GetAllRoles();
        }
    }
}
