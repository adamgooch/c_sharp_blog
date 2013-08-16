using System;
using System.Collections.Generic;
using System.Web;
using Application;
using Application.Users;
using Moq;
using NUnit.Framework;
using Tests;

namespace UserInteractorTests
{
    [TestFixture]
    [Category("Interactors > User > Creation")]
    public class When_creating_a_user
        : TestBase<UserInteractor>
    {
        private User mockCreatedUser;
        private const string TestUserEmail = "text@example.com";
        private const string TestUserPassword = "testPassword";
        private readonly byte[] mockGeneratedSalt = new byte[16];
        private readonly byte[] mockGeneratedPasswordDigest = new byte[16];

        public override void Arrange()
        {
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.CreateUser( It.IsAny<User>() ) )
                  .Callback( ( User user ) => mockCreatedUser = user );
            Mocks.GetMock<IAuthenticator>()
                  .Setup( x => x.GenerateSalt() )
                  .Returns( mockGeneratedSalt );
            Mocks.GetMock<IAuthenticator>()
                  .Setup( x => x.GeneratePasswordDigest( It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<int>() ) )
                  .Returns( mockGeneratedPasswordDigest );
        }

        public override void Act()
        {
            ClassUnderTest.CreateUser( TestUserEmail, TestUserPassword );
        }

        [Test]
        public void it_creates_a_user_in_a_repository()
        {
            Mocks.GetMock<IUserRepository>()
                  .Verify( x => x.CreateUser( It.IsAny<User>() ), Times.Once() );
        }

        [Test]
        public void it_creates_a_user_with_the_given_email()
        {
            Assert.AreEqual( TestUserEmail, mockCreatedUser.Email );
        }

        [Test]
        public void it_creates_a_user_with_a_new_salt()
        {
            Assert.AreEqual( mockGeneratedSalt, mockCreatedUser.Salt );
            Mocks.GetMock<IAuthenticator>()
                  .Verify( x => x.GenerateSalt(), Times.Once() );
        }

        [Test]
        public void it_creates_a_user_with_a_password_digest()
        {
            Assert.AreEqual( mockGeneratedPasswordDigest, mockCreatedUser.PasswordDigest );
            Mocks.GetMock<IAuthenticator>()
                  .Verify( x => x.GeneratePasswordDigest( TestUserPassword, mockGeneratedSalt, 5000 ), Times.Once() );
        }

        [Test]
        public void it_creates_a_user_with_the_default_role()
        {
            Assert.AreEqual( Role.Default, mockCreatedUser.Role );
        }

        [Test]
        public void it_creates_a_user_with_a_new_token()
        {
            Assert.AreNotEqual( Guid.Empty, mockCreatedUser.VerifiedToken );
        }
    }
    
    [TestFixture]
    [Category("Interactors > User > Verification")]
    public class When_verifying_a_user
        : TestBase<UserInteractor>
    {
        private User testUser;
        private readonly Guid testVerifiedToken = Guid.NewGuid();

        public override void Arrange()
        {
            testUser = new User { VerifiedToken = testVerifiedToken };
            var allUsers = new List<User> { testUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );
        }

        public override void Act()
        {
            ClassUnderTest.VerifyUser(testVerifiedToken);
        }

        [Test]
        public void it_clears_the_users_verified_token()
        {
            
            Assert.AreEqual( Guid.Empty, testUser.VerifiedToken );
        }

        [Test]
        public void it_saves_the_user_when_verified()
        {
            Mocks.GetMock<IUserRepository>()
                  .Verify( x => x.SaveUser( testUser ), Times.Once() );
        }
    }

    [TestFixture]
    [Category("Interactors > User > GetByUsername")]
    public class When_getting_a_user_by_username
        : TestBase<UserInteractor>
    {
        private const string TestUserEmail = "text@example.com";
        private readonly byte[] testUserSalt = new byte[16];
        private User returnedUser;
        
        public override void Arrange()
        {
            var testUser = new User { Salt = testUserSalt, Email = TestUserEmail };
            var allUsers = new List<User> { testUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );
        }

        public override void Act()
        {
            returnedUser = ClassUnderTest.GetUserByUsername( TestUserEmail );
        }

        [Test]
        public void it_gets_a_user_by_username()
        {
            Assert.AreEqual(testUserSalt, returnedUser.Salt);
        }
    }
    
    [TestFixture]
    [Category( "Interactors > User > GetById" )]
    public class When_getting_a_user_by_id
        : TestBase<UserInteractor>
    {
        private User returnedUser;
        private readonly Guid testUserId = Guid.NewGuid();
        private readonly byte[] testUserSalt = new byte[16];

        public override void Arrange()
        {
            var testUser = new User { Salt = testUserSalt, Id = testUserId };
            var allUsers = new List<User> { testUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );
        }

        public override void Act()
        {
            returnedUser = ClassUnderTest.GetUserById( testUserId );
        }

        [Test]
        public void it_gets_a_user_by_id()
        {
            Assert.AreEqual(testUserSalt, returnedUser.Salt);
            Assert.AreEqual( testUserId, returnedUser.Id );
        }
    }

    [TestFixture]
    [Category( "Interactors > User > GetByCookie" )]
    public class When_getting_a_user_by_cookie
        : TestBase<UserInteractor>
    {
        private User returnedUser;
        private readonly HttpCookie TestCookie = new HttpCookie( "TestCookie" );
        private readonly Guid testUserId = Guid.NewGuid();
        private readonly byte[] testUserSalt = new byte[16];

        public override void Arrange()
        {
            TestCookie.Values["Id"] = testUserId.ToString();
            TestCookie.Values["Salt"] = System.Text.Encoding.Default.GetString( testUserSalt );
            var testUser = new User { Salt = testUserSalt, Id = testUserId };
            var allUsers = new List<User> { testUser };
            Mocks.GetMock<IUserRepository>()
                    .Setup( x => x.GetAllUsers() )
                    .Returns( allUsers );
            Mocks.GetMock<IAuthenticator>()
                    .Setup( x => x.DecryptAuthenticationCookie( TestCookie ) )
                    .Returns( TestCookie );
        }

        public override void Act()
        {
            returnedUser = ClassUnderTest.GetUserByCookie( TestCookie );
        }

        [Test]
        public void it_gets_a_user_by_cookie()
        {
            Assert.AreEqual(testUserSalt, returnedUser.Salt);
            Assert.AreEqual(testUserId, returnedUser.Id);
        }
    }
}
