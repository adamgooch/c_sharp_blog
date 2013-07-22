using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Application;

namespace Tests.Application
{
    [TestFixture]
    public class AuthenticatorTest
    {
        private Authenticator sut;
        const string PASSWORD = "PASSWORD";
        const string PASSWORD_2 = "PASSWORD_2";

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
            var digest1 = sut.GeneratePasswordDigest( PASSWORD, salt1, 5000 );
            var digest2 = sut.GeneratePasswordDigest( PASSWORD, salt2, 5000 );
            Assert.AreNotEqual( digest1, digest2 );
        }

        [Test]
        public void it_generates_a_unique_password_digest_given_different_password()
        {
            var salt = sut.GenerateSalt();
            var digest1 = sut.GeneratePasswordDigest( PASSWORD, salt, 5000 );
            var digest2 = sut.GeneratePasswordDigest( PASSWORD_2, salt, 5000 );
            Assert.AreNotEqual( digest1, digest2 );
        }

        [Test]
        public void it_generates_a_the_same_password_digest_given_the_same_password_and_salt()
        {
            var salt = sut.GenerateSalt();
            var digest1 = sut.GeneratePasswordDigest( PASSWORD, salt, 5000 );
            var digest2 = sut.GeneratePasswordDigest( PASSWORD, salt, 5000 );
            Assert.AreEqual( digest1, digest2 );
        }

        [Test]
        public void it_is_authenticated_when_the_password_is_correct()
        {
            var salt = sut.GenerateSalt();
            var digest1 = sut.GeneratePasswordDigest( PASSWORD, salt, 5000 );
            var authenticated = sut.Authenticate( PASSWORD, salt, digest1, 5000 );
            Assert.IsTrue( authenticated );
        }

        [Test]
        public void it_is_not_authenticated_when_the_password_is_incorrect()
        {
            var salt = sut.GenerateSalt();
            var digest1 = sut.GeneratePasswordDigest( PASSWORD, salt, 5000 );
            var authenticated = sut.Authenticate( PASSWORD_2, salt, digest1, 5000 );
            Assert.IsFalse( authenticated );
        }

        [Test]
        public void it_generates_an_encrypted_authentication_cookie()
        {
            var id = Guid.NewGuid();
            var salt = sut.GenerateSalt();
            var cookie = sut.GenerateAuthenticationCookie( id, salt );
            Assert.AreNotEqual( cookie.Value, id.ToString() );
        }

        [Test]
        public void it_decrypts_an_authentication_cookie()
        {
            var id = Guid.NewGuid();
            var salt = sut.GenerateSalt();
            var cookie = sut.GenerateAuthenticationCookie( id, salt );
            var decryptedCookie = sut.DecryptAuthenticationCookie( cookie );
            Assert.AreEqual( id.ToString(), decryptedCookie.Values["Id"] );
            Assert.AreEqual( salt, System.Text.Encoding.Default.GetBytes( decryptedCookie.Values["Salt"] ) );
        }

        [Test]
        public void it_creates_an_http_only_authentication_cookie()
        {
            var id = Guid.NewGuid();
            var salt = sut.GenerateSalt();
            var cookie = sut.GenerateAuthenticationCookie( id, salt );
            Assert.IsTrue( cookie.HttpOnly );
        }
    }

    internal sealed class HttpSessionMock : HttpSessionStateBase
    {
        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _collection.Keys; }
        }

        public override string SessionID
        {
            get { return "1"; }
        }

        private readonly NameValueCollection _collection = new NameValueCollection();
    }
}
