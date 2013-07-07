using System;
using System.Linq;
using Application;
using Application.Users;
using Data.Repositories;
using NUnit.Framework;

namespace Tests.Data
{
    [TestFixture]
    public class SQLServerUserRepositoryTest
    {
        private SQLServerUserRepository sut;
        private const string email = "test@example.com";

        [SetUp]
        public void Setup()
        {
            sut = new SQLServerUserRepository();
        }

        [Test]
        public void it_creates_a_user()
        {
            var user = new User
                {
                    Email = email,
                    Salt = new byte[16],
                    PasswordDigest = new byte[32],
                    CreatedDate = DateTime.Today,
                    VerifiedToken = Guid.NewGuid(),
                    Role = Roles.Default()
                };
            sut.CreateUser( user );
            var allUsers = sut.GetAllUsers();
            Assert.AreEqual( 1, allUsers.Count() );
            sut.DeleteByEmail( email );
        }
    }
}
