using System;
using System.Linq;
using Application.Utility;
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
        private Guid id;

        [SetUp]
        public void Setup()
        {
            sut = new SqlServerUserRepository();
        }

        [TearDown]
        public void TearDown()
        {
            sut.DeleteUser( id );
        }

        [Test]
        public void it_creates_a_user()
        {
            var salt = PBKDF2Helper.GenerateSalt();
            var passDigest = PBKDF2Helper.GeneratePasswordDigest( "password", salt );
            var success = sut.CreateUser( Email, passDigest, salt, Guid.NewGuid()  );
            var allUsers = sut.GetAllUsers();
            id = allUsers.FirstOrDefault().Id;
            Assert.IsTrue( success );
            Assert.AreEqual( 1, allUsers.Count() );
        }
        
        [Test]
        public void it_maps_a_user_correctly()
        {
            var salt = PBKDF2Helper.GenerateSalt();
            var passDigest = PBKDF2Helper.GeneratePasswordDigest( "password", salt );
            var success = sut.CreateUser( Email, passDigest, salt, Guid.NewGuid() );
            var allUsers = sut.GetAllUsers();
            var mappedUser = allUsers.First();
            id = mappedUser.Id;
            Assert.AreEqual( Email, mappedUser.Email, "Email was not mapped correctly" );
            Assert.AreEqual( salt, mappedUser.Salt, "Salt was not mapped correctly" );
            Assert.AreEqual( passDigest, mappedUser.PasswordDigest, "PasswordDigest was not mapped correctly" );
            Assert.AreEqual( false, mappedUser.Active, "Active was not mapped correctly" );
        }
    }
}
