using System;
using System.IO;
using Application;
using Application.Users;
using AutoMoq;
using Moq;
using NUnit.Framework;

namespace Tests.Application.Interactors
{
    [TestFixture]
    public class UserInteractorTest
    {
        private AutoMoqer mocker;
        private UserInteractor sut;

        [SetUp]
        public void Setup()
        {
            mocker = new AutoMoqer();
            sut = mocker.Resolve<UserInteractor>();
        }

        [Test]
        public void it_creates_a_user()
        {
            sut.CreateUser( "test@example.com", "testPassword" );
            mocker.GetMock<IUserRepository>()
                  .Verify( x => x.CreateUser( It.IsAny<User>() ), Times.Once() );
        }

        [Test]
        public void it_creates_a_user_with_the_given_email()
        {
            var createdUser = new User();
            mocker.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Callback( ( User user ) => createdUser = user );
            sut.CreateUser( "test@example.com", "testPassword" );
            Assert.AreEqual( "test@example.com", createdUser.Email );
        }

        [Test]
        public void it_creates_a_user_with_a_unique_salt()
        {
            var createdUser = new User();
            mocker.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Callback( ( User user ) => createdUser = user );
            sut.CreateUser( "test@example.com", "testPassword" );
            Assert.NotNull( createdUser.Salt );
            mocker.GetMock<IAuthenticator>()
                  .Verify( x => x.GenerateSalt(), Times.Once() );
        }

        [Test]
        public void it_creates_a_user_with_a_password_digest()
        {
            var createdUser = new User();
            var generatedSalt = new byte[16];
            mocker.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Callback( ( User user ) => createdUser = user );
            mocker.GetMock<IAuthenticator>()
                  .Setup( x => x.GenerateSalt() )
                  .Returns( generatedSalt );
            mocker.GetMock<IAuthenticator>()
                  .Setup( x => x.GeneratePasswordDigest( It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<int>() ) )
                  .Returns( "Anything" );
            sut.CreateUser( "test@example.com", "testPassword" );
            Assert.NotNull( createdUser.PasswordDigest );
            mocker.GetMock<IAuthenticator>()
                  .Verify( x => x.GeneratePasswordDigest( "testPassword", generatedSalt, 5000 ), Times.Once() );
        }

        [Test]
        public void it_creates_a_user_with_todays_date()
        {
            var createdUser = new User();
            var today = DateTime.Today;
            mocker.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Callback( ( User user ) => createdUser = user );
            sut.CreateUser( "test@example.com", "testPassword" );
            Assert.AreEqual( today, createdUser.CreatedDate );
        }

        [Test]
        public void it_creates_a_user_with_the_default_role()
        {
            var createdUser = new User();
            mocker.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Callback( ( User user ) => createdUser = user );
            sut.CreateUser( "test@example.com", "testPassword" );
            Assert.AreEqual( Roles.Default(), createdUser.Role );
        }

        [Test]
        public void it_creates_a_user_with_a_new_token()
        {
            var createdUser = new User();
            mocker.GetMock<IUserRepository>()
                 .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                 .Callback( ( User user ) => createdUser = user );
            sut.CreateUser( "test@example.com", "testPassword" );
            Assert.AreNotEqual( Guid.Empty, createdUser.VerifiedToken );
        }

        [Test]
        public void it_throws_an_exception_when_a_user_fails_to_be_created()
        {
            mocker.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Throws( new IOException() );
            var ex = Assert.Throws<IOException>(() => sut.CreateUser( "test@example.com", "testPassword" ) );
        }
    }
}
