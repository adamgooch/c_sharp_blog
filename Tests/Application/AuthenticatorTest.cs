using NUnit.Framework;
using Application;

namespace Tests.Application
{
    [TestFixture]
    public class AuthenticatorTest
    {
        private Authenticator sut;

        [SetUp]
        public void Setup()
        {
            sut = new Authenticator();
        }

        [Test]
        public void it_generates_a_unique_salt()
        {
            var salt1 = sut.GenerateSalt();
            var salt2 = sut.GenerateSalt();
            Assert.AreNotEqual( salt1, salt2 );
        }

        [Test]
        public void it_generates_a_unique_password_digest_given_different_salt()
        {
            var salt1 = sut.GenerateSalt();
            var salt2 = sut.GenerateSalt();
            var password = "password";
            var digest1 = sut.GeneratePasswordDigest( password, salt1, 5000 );
            var digest2 = sut.GeneratePasswordDigest(password, salt2, 5000);
            Assert.AreNotEqual( digest1, digest2 );
        }

        [Test]
        public void it_generates_a_unique_password_digest_given_different_password()
        {
            var salt = sut.GenerateSalt();
            var password = "password";
            var password2 = "password2";
            var digest1 = sut.GeneratePasswordDigest( password, salt, 5000 );
            var digest2 = sut.GeneratePasswordDigest( password2, salt, 5000 );
            Assert.AreNotEqual( digest1, digest2 );
        }

        [Test]
        public void it_generates_a_the_same_password_digest_given_the_same_password_and_salt()
        {
            var salt = sut.GenerateSalt();
            var password = "password";
            var digest1 = sut.GeneratePasswordDigest( password, salt, 5000 );
            var digest2 = sut.GeneratePasswordDigest( password, salt, 5000 );
            Assert.AreEqual( digest1, digest2 );
        }
    }
}
