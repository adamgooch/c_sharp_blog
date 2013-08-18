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
    [Category( "Interactors > User > Creation" )]
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
    [Category( "Interactors > User > Verification" )]
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
            ClassUnderTest.VerifyUser( testVerifiedToken );
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
    [Category( "Interactors > User > GetByUsername" )]
    public class When_getting_a_user_by_username
        : TestBase<UserInteractor>
    {
        private const string TestUserEmail = "text@example.com";
        private const string WrongUserEmail = "wrong@example.com";
        private readonly byte[] testUserSalt = new byte[] { 1, 2 };
        private readonly byte[] wrongUserSalt = new byte[] { 3, 4 };
        private User testUser;
        private User wrongUser;

        public override void Arrange()
        {
            testUser = new User { Salt = testUserSalt, Email = TestUserEmail };
            wrongUser = new User { Salt = wrongUserSalt, Email = WrongUserEmail };
        }

        [Test]
        public void it_gets_a_user_by_username()
        {
            var allUsers = new List<User> { wrongUser, testUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );

            var returnedUser = ClassUnderTest.GetUserByUsername( TestUserEmail );

            Assert.AreEqual( testUserSalt, returnedUser.Salt );
        }

        [Test]
        public void it_returns_null_if_no_user_is_found_with_the_given_username()
        {
            var allUsers = new List<User> { wrongUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );

            var returnedUser = ClassUnderTest.GetUserByUsername( TestUserEmail );

            Assert.IsNull( returnedUser );
        }
    }

    [TestFixture]
    [Category( "Interactors > User > GetById" )]
    public class When_getting_a_user_by_id
        : TestBase<UserInteractor>
    {
        private User returnedUser;
        private User testUser;
        private User wrongUser;
        private IEnumerable<User> allUsers;
        private readonly Guid testUserId = Guid.NewGuid();
        private readonly byte[] testUserSalt = new byte[] { 1, 2 };

        public override void Arrange()
        {
            testUser = new User { Salt = testUserSalt, Id = testUserId };
            wrongUser = new User { Salt = new byte[] { 3, 4 } };
        }

        [Test]
        public void it_gets_a_user_by_id()
        {
            allUsers = new List<User> { wrongUser, testUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );

            returnedUser = ClassUnderTest.GetUserById( testUserId );

            Assert.AreEqual( testUserSalt, returnedUser.Salt );
            Assert.AreEqual( testUserId, returnedUser.Id );
        }

        [Test]
        public void it_returns_null_if_no_user_is_found_with_the_given_id()
        {
            allUsers = new List<User> { wrongUser };
            Mocks.GetMock<IUserRepository>()
                  .Setup( x => x.GetAllUsers() )
                  .Returns( allUsers );

            returnedUser = ClassUnderTest.GetUserById( testUserId );

            Assert.IsNull( returnedUser );
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
        private readonly byte[] testUserSalt = new byte[] { 1, 2 };
        private readonly byte[] wrongSalt = new byte[] { 3, 4 };
        private IEnumerable<User> allUsers;
        private User wrongUser;

        public override void Arrange()
        {
            TestCookie.Values["Id"] = testUserId.ToString();
            var testUser = new User { Salt = testUserSalt, Id = testUserId };
            wrongUser = new User { Salt = wrongSalt, Id = Guid.NewGuid() };
            allUsers = new List<User> { wrongUser, testUser };
            Mocks.GetMock<IAuthenticator>()
                    .Setup( x => x.DecryptAuthenticationCookie( TestCookie ) )
                    .Returns( TestCookie );
        }

        [Test]
        public void it_gets_a_user_by_cookie()
        {
            TestCookie.Values["Salt"] = System.Text.Encoding.Default.GetString( testUserSalt );
            Mocks.GetMock<IUserRepository>()
                    .Setup( x => x.GetAllUsers() )
                    .Returns( allUsers );

            returnedUser = ClassUnderTest.GetUserByCookie( TestCookie );

            Assert.AreEqual( testUserSalt, returnedUser.Salt );
            Assert.AreEqual( testUserId, returnedUser.Id );
        }

        [Test]
        public void it_returns_null_if_no_user_is_found_that_matches_the_cookie()
        {
            TestCookie.Values["Salt"] = System.Text.Encoding.Default.GetString( testUserSalt );
            Mocks.GetMock<IUserRepository>()
                    .Setup( x => x.GetAllUsers() )
                    .Returns( new List<User>() { wrongUser } );

            returnedUser = ClassUnderTest.GetUserByCookie( TestCookie );

            Assert.IsNull( returnedUser );
        }

        [Test]
        public void it_returns_null_if_the_salt_in_the_cookie_does_not_match_the_id()
        {
            TestCookie.Values["Salt"] = System.Text.Encoding.Default.GetString( wrongSalt );
            Mocks.GetMock<IUserRepository>()
                    .Setup( x => x.GetAllUsers() )
                    .Returns( allUsers );

            returnedUser = ClassUnderTest.GetUserByCookie( TestCookie );

            Assert.IsNull( returnedUser );
        }
    }

    [TestFixture]
    [Category( "Interactors > User > GetAll" )]
    public class When_getting_all_users
        : TestBase<UserInteractor>
    {
        private IEnumerable<User> returnedUsers;
        private IEnumerable<User> testUsers;

        public override void Arrange()
        {
            var user1 = new User();
            var user2 = new User();
            testUsers = new List<User> { user1, user2 };
            Mocks.GetMock<IUserRepository>()
                 .Setup( x => x.GetAllUsers() )
                 .Returns( testUsers );
        }

        public override void Act()
        {
            returnedUsers = ClassUnderTest.GetAllUsers();
        }

        [Test]
        public void it_gets_all_users()
        {
            Assert.AreEqual( testUsers, returnedUsers );
        }
    }

    [TestFixture]
    [Category( "Interactors > User > DeleteById" )]
    public class When_deleting_a_user_by_id
        : TestBase<UserInteractor>
    {
        private Guid id;

        public override void Arrange()
        {
            id = Guid.NewGuid();
        }

        public override void Act()
        {
            ClassUnderTest.DeleteById( id );
        }

        [Test]
        public void it_deletes_the_user_with_the_given_id()
        {
            Mocks.GetMock<IUserRepository>()
                .Verify( x => x.DeleteById( id ) );
        }
    }

    [TestFixture]
    [Category( "Interactors > User > EditRole" )]
    public class When_editing_a_users_role
        : TestBase<UserInteractor>
    {
        private readonly Guid id = Guid.NewGuid();
        private const Role NewRole = Role.Author;
        private User savedUser;

        public override void Arrange()
        {
            var originalUser = new User() { Id = id, Role = Role.Default };
            var wrongUser = new User() { Id = Guid.NewGuid() };
            var allUsers = new List<User>() { wrongUser, originalUser };
            Mocks.GetMock<IUserRepository>()
                 .Setup( x => x.GetAllUsers() )
                 .Returns( allUsers );
            Mocks.GetMock<IUserRepository>()
                 .Setup( x => x.SaveUser( It.IsAny<User>() ) )
                 .Callback( ( User user ) => savedUser = user );
        }

        [Test]
        public void it_edits_the_users_role()
        {
            ClassUnderTest.EditRole( id, NewRole );
            Assert.AreEqual( NewRole, savedUser.Role, "The role was set incorrectly" );
            Assert.AreEqual( id, savedUser.Id, "The role was changed on the wrong user" );
        }

        [Test]
        public void it_throws_an_error_when_given_an_invalid_id()
        {
            Assert.Throws<InvalidUserIdException>( () => ClassUnderTest.EditRole( Guid.NewGuid(), NewRole ) );
        }
    }
}
