using System;

namespace Application.Users
{
    public class UserInteractor
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
                    CreatedDate = DateTime.Today,
                    Role = Roles.Default(),
                    VerifiedToken = Guid.NewGuid()
                };
            repository.CreateUser( user );
        }
    }
}
