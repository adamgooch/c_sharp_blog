using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Users
{
    public class UserInteractor : IUserInteractor
    {
        private readonly IUserRepository repository;
        private readonly IAuthenticator authenticator;
        private readonly IMailer mailer;
        private const int Iterations = 5000;

        public UserInteractor( IUserRepository userRepository, IAuthenticator authenticator, IMailer mailer )
        {
            repository = userRepository;
            this.authenticator = authenticator;
            this.mailer = mailer;
        }

        public void CreateUser( string email, string password )
        {
            var salt = authenticator.GenerateSalt();
            var user = new User
                {
                    Email = email,
                    Salt = salt,
                    PasswordDigest = authenticator.GeneratePasswordDigest( password, salt, Iterations ),
                    Role = Role.Default,
                    VerifiedToken = Guid.NewGuid()
                };
            try
            {
                repository.CreateUser( user );
                mailer.SendNewUserVerificationEmail( user.Email, user.VerifiedToken );
            }
            catch( Exception )
            {
                throw;
            }
        }

        public void VerifyUser( Guid token )
        {
            var users = repository.GetAllUsers();
            var user = users.Where( u => u.VerifiedToken == token );
            var theUser = user.First();
            theUser.VerifiedToken = Guid.Empty;
            repository.SaveUser( theUser );
        }

        public User GetUserByUsername( string username )
        {
            var users = repository.GetAllUsers();
            var user = users.Where( x => x.Email == username );
            return user.First();
        }

        public User GetUserById( Guid id )
        {
            var users = repository.GetAllUsers();
            var user = users.Where( u => u.Id == id );
            return user.First();
        }

        public User GetUserByCookie( HttpCookie cookie )
        {
            var decryptedCookie = authenticator.DecryptAuthenticationCookie( cookie );
            var userId = new Guid( decryptedCookie.Values["Id"] );
            var userSalt = System.Text.Encoding.Default.GetBytes( decryptedCookie.Values["Salt"] );
            var user = GetUserById( userId );
            if( user != null && userSalt.SequenceEqual( user.Salt ) ) return user;
            return null;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return repository.GetAllUsers();
        }

        public void DeleteById( Guid id )
        {
            repository.DeleteById( id );
        }

        public void EditRole( Guid id, Role role )
        {
            var user = repository.GetAllUsers().Where( p => p.Id == id );
            user.First().Role = role;
            repository.SaveUser( user.First() );
        }
    }
}
