using System;
using System.Linq;

namespace Application.Users
{
    public class UserInteractor : IUserInteractor
    {
        private readonly IUserRepository repository;
        private readonly IAuthenticator authenticator;
        private const int ITERATIONS = 5000;

        public UserInteractor( IUserRepository userRepository, IAuthenticator authenticator )
        {
            repository = userRepository;
            this.authenticator = authenticator;
        }

        public void CreateUser( string email, string password )
        {
            var salt = authenticator.GenerateSalt();
            var user = new User
                {
                    Email = email,
                    Salt = salt,
                    PasswordDigest = authenticator.GeneratePasswordDigest( password, salt, ITERATIONS ),
                    Role = Roles.Default(),
                    VerifiedToken = Guid.NewGuid()
                };
            repository.CreateUser( user );
            authenticator.SendNewUserVerificationEmail( user );
        }

        public void VerifyUser( Guid token )
        {
            var users = repository.GetAllUsers();
            var user = from u in users where u.VerifiedToken == token select u;
            var theUser = user.First();
            theUser.VerifiedToken = Guid.Empty;
            repository.SaveUser( theUser );
        }
    }
}
