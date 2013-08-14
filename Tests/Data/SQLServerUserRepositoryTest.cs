using System;
using System.Linq;
using Application;
using Application.Users;
using Data.Repositories;
using NUnit.Framework;

namespace Tests.Data
{
    [TestFixture]
    public class SqlServerUserRepositoryTest
    {
        private SqlServerUserRepository sut;
        private const string Email = "test@example.com";
        private User user;

        [SetUp]
        public void Setup()
        {
            sut = new SqlServerUserRepository();
            var authenticator = new Authenticator();
            var salt = authenticator.GenerateSalt();
            user = new User
            {
                Email = Email,
                Salt = salt,
                PasswordDigest = authenticator.GeneratePasswordDigest( "password", salt, 5000 ),
                VerifiedToken = Guid.NewGuid(),
                Role = Role.Default
            };
        }

        [TearDown]
        public void TearDown()
        {
            sut.DeleteByEmail( Email );
        }

        [Test]
        public void it_creates_a_user()
        {
            sut.CreateUser( user );
            var allUsers = sut.GetAllUsers();
            Assert.AreEqual( 1, allUsers.Count() );
        }

        [Test]
        public void it_maps_a_user_correctly()
        {
            sut.CreateUser( user );
            var mappedUser = sut.GetAllUsers().First();
            Assert.AreEqual( user.Email, mappedUser.Email, "Email was not mapped correctly" );
            Assert.AreEqual( user.Salt, mappedUser.Salt, "Salt was not mapped correctly" );
            Assert.AreEqual( user.PasswordDigest, mappedUser.PasswordDigest, "PasswordDigest was not mapped correctly" );
            Assert.AreEqual( user.VerifiedToken, mappedUser.VerifiedToken, "VerifiedToken was not mapped correctly" );
            Assert.AreEqual( user.Role, mappedUser.Role, "Role was not mapped correctly" );
        }

        [Test]
        public void it_saves_a_current_user()
        {
            sut.CreateUser( user );
            var createdUser = sut.GetAllUsers().First();
            createdUser.VerifiedToken = Guid.Empty;

            sut.SaveUser( createdUser );
            var allUsers = sut.GetAllUsers();
            var savedUser = allUsers.First();

            Assert.AreEqual( 1, allUsers.Count() );
            Assert.AreEqual( createdUser.Id, savedUser.Id );
            Assert.AreEqual( createdUser.VerifiedToken, savedUser.VerifiedToken );
        }

        [Test]
        public void it_deletes_by_id()
        {
            sut.CreateUser( user );
            var createdUser = sut.GetAllUsers().First();
            sut.DeleteById( createdUser.Id );
            Assert.AreEqual( 0, sut.GetAllUsers().Count() );
        }

        [Test]
        public void it_deletes_by_email()
        {
            sut.CreateUser( user );
            var createdUser = sut.GetAllUsers().First();
            sut.DeleteByEmail( createdUser.Email );
            Assert.AreEqual( 0, sut.GetAllUsers().Count() );
        }
    }
}
