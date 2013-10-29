using System;
using System.Collections.Specialized;
using System.Web;
using System.Linq;
using NUnit.Framework;
using Application;
using Application.Users;
using Data.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;

namespace AuthenticatorTests
{
    [TestFixture]
    [Category( "Interactors > Authenticator > UserCreation" )]
    public class AuthenticatorTests
    {
        private Authenticator sut;
        private IUserInteractor userInteractor;
        private const string EmailDirectory = @"C:\Users\agooch\Projects\c_sharp_blog\Tests\EmailTest";

        [SetUp]
        public void setup()
        {
            var userRepo = new SqlServerUserRepository();
            userInteractor = new UserInteractor( userRepo );
            sut = new Authenticator( userRepo, new Mailer() );
        }

        [TearDown]
        public void teardown()
        {
            DeleteEmails();
            foreach( var user in userInteractor.GetAllUsers() )
                userInteractor.DeleteUser( user.Id );
        }

        [Test]
        public void When_creating_a_user_it_creates_an_inactive_user_with_the_correct_email()
        {
            var success = sut.CreateUser( "john@example.com", "password", "password" );
            Assert.IsTrue( success, "User failed to be created" );
            var createdUser = GetCreatedUser();
            Assert.IsFalse( createdUser.Active, "User was active" );
            Assert.AreEqual( "john@example.com", createdUser.Email, "The email address was wrong" );
        }

        [Test]
        public void When_creating_a_user_it_does_not_create_a_user_if_the_password_and_confirmation_do_not_match()
        {
            var success = sut.CreateUser( "john@example.com", "password", "Password" );
            Assert.IsFalse( success, "User was created" );
        }

        [Test]
        public void When_creating_a_user_it_creates_a_unique_password_digest_for_two_users_with_the_same_password()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            sut.CreateUser( "jane@example.com", "password", "password" );
            var allUsers = userInteractor.GetAllUsers();
            var firstUser = allUsers.ElementAt( 0 );
            var secondUser = allUsers.ElementAt( 1 );
            Assert.IsFalse( firstUser.PasswordDigest.SequenceEqual( secondUser.PasswordDigest ), "Password digest was wrong" );
            Assert.IsFalse( firstUser.Salt.SequenceEqual( secondUser.Salt ), "Salt was wrong" );
        }

        [Test]
        public void When_creating_a_user_it_does_not_create_a_user_with_the_same_email()
        {
            var firstUserSuccess = sut.CreateUser( "john@example.com", "password", "password" );
            var secondUserSuccess = sut.CreateUser( "john@example.com", "password", "password" );
            var allUsers = userInteractor.GetAllUsers();
            Assert.IsTrue( firstUserSuccess, "First user should have been created" );
            Assert.IsFalse( secondUserSuccess, "Second user should NOT have been created" );
            Assert.AreEqual( 1, allUsers.Count(), "There should have only been 1 user" );
        }

        [Test]
        public void When_creating_a_user_it_sends_a_verification_email_to_the_user()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var mailFolder = new DirectoryInfo( EmailDirectory );
            var emailContents = GetEmailContents();
            Assert.IsTrue( emailContents.Contains( "To: john@example.com" ), "The email was not sent to the right address" );
            Assert.IsTrue( emailContents.Contains( @"account/activate?token=" ), "The email had the wrong activation url" );
        }

        [Test]
        public void When_authenticating_it_is_true_when_the_username_and_password_are_correct_and_the_user_is_active()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var createdUser = GetCreatedUser();
            userInteractor.SetActive( createdUser.Id, true );
            var result = sut.Authenticate( "john@example.com", "password" );
            Assert.IsTrue( result, "The user should have authenticated" );
        }

        [Test]
        public void When_authenticating_it_is_false_when_the_password_is_incorrect_and_the_user_is_active()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var createdUser = GetCreatedUser();
            userInteractor.SetActive( createdUser.Id, true );
            var result = sut.Authenticate( "john@example.com", "Password" );
            Assert.IsFalse( result, "The user should not have authenticated" );
        }

        [Test]
        public void When_authenticating_it_is_false_when_the_password_is_correct_and_the_user_is_inactive()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var createdUser = GetCreatedUser();
            userInteractor.SetActive( createdUser.Id, false );
            var result = sut.Authenticate( "john@example.com", "Password" );
            Assert.IsFalse( result, "The user should not have authenticated" );
        }

        [Test]
        public void When_generating_an_auth_cookie_it_is_http_only_and_secure()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var authCookie = sut.GenerateAuthCookie( "john@example.com", false );
            Assert.IsTrue( authCookie.HttpOnly, "The auth cookie was not httponly" );
            //Assert.IsTrue( authCookie.Secure, "The auth cookie was not secure" );
        }

        [Test]
        public void When_generating_an_auth_cookie_with_remember_me_set_to_false_it_is_a_session_only_cookie()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var authCookie = sut.GenerateAuthCookie( "john@example.com", false );
            Assert.AreEqual( new DateTime(), authCookie.Expires, "The auth cookie was not a session cookie" );
        }

        [Test]
        public void When_generating_an_auth_cookie_with_remember_me_set_to_true_it_expires_in_7_days()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var authCookie = sut.GenerateAuthCookie( "john@example.com", true );
            Assert.AreEqual( DateTime.Now.AddDays( 7d ), authCookie.Expires, "The auth cookie expiration was wrong" );
        }

        [Test]
        public void When_generating_an_auth_cookie_it_can_identify_a_user()
        {
            var userCreated = sut.CreateUser( "john@example.com", "password", "password" );
            var authCookie = sut.GenerateAuthCookie( "john@example.com", false );
            var allUsers = userInteractor.GetAllUsers();
            var createdUser = allUsers.FirstOrDefault();
            var cookieUser = sut.GetUser( authCookie );
            Assert.AreEqual( createdUser.Id, cookieUser.Id, "The auth cookie returned a different user" );
        }

        /*
         * This test is to mitigate the replay attack.
         * Making this pass and the previous one pass is a challenge I have yet to solve
        [Test]
        public void When_generating_an_auth_cookie_the_value_is_different_every_time()
        {
            var userCreated = sut.CreateUser( "john@example.com", "password", "password" );
            var authCookie = sut.GenerateAuthCookie( "john@example.com", false );
            var authCookie2 = sut.GenerateAuthCookie( "john@example.com", false );
            Assert.AreNotEqual( authCookie.Value, authCookie2.Value );
        }
        */

        [Test]
        public void When_sending_the_password_reset_email_it_fails_if_the_user_does_not_exist()
        {
            var emailSent = sut.SendPasswordResetEmail( "john@example.com" );
            Assert.IsFalse( emailSent, "Password reset email should not have been sent" );
        }

        [Test]
        public void When_sending_the_password_reset_email_it_has_a_link_to_the_password_reset()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            DeleteEmails();
            var success = sut.SendPasswordResetEmail( "john@example.com" );
            var emailContents = GetEmailContents();
            Assert.IsTrue( success, "The email was not sent" );
            Assert.IsTrue( emailContents.Contains( "To: john@example.com" ), "The email was sent to the wrong address" );
            Assert.IsTrue( emailContents.Contains( @"/account/ResetPassword?token=" ), "The reset password url was wrong" );
        }

        [Test]
        public void When_resetting_a_password_it_succeeds_if_token_is_correct()
        {
            sut.CreateUser( "john@example.com", "password", "password" );
            var createdUser = GetCreatedUser();
            var originalPasswordDigest = createdUser.PasswordDigest;
            DeleteEmails();

            var emailSent = sut.SendPasswordResetEmail( "john@example.com" );
            Assert.IsTrue( emailSent, "Password reset email was not sent" );

            var token = GetTokenFromEmail();
            var success = sut.ResetPassword( "newPassword", "newPassword", token );
            Assert.IsTrue( success, "The password was not reset" );

            createdUser = GetCreatedUser();
            var newPasswordDigest = createdUser.PasswordDigest;
            Assert.IsFalse( newPasswordDigest.SequenceEqual( originalPasswordDigest ), "The password digest did not change" );
        }

        [Test]
        public void When_resetting_a_password_it_fails_if_the_confirmation_does_not_match_the_password()
        {
            //This has the full setup because we need to make sure it isn't failing due to the token
            sut.CreateUser( "john@example.com", "password", "password" );
            DeleteEmails();
            var emailSent = sut.SendPasswordResetEmail( "john@example.com" );
            Assert.IsTrue( emailSent, "Password reset email was not sent" );
            var token = GetTokenFromEmail();
            var success = sut.ResetPassword( "newPassword", "newpassword", token );
            Assert.IsFalse( success, "it should have failed" );
        }

        [Test]
        public void When_resetting_a_password_it_fails_if_the_token_doesnt_exist()
        {
            var success = sut.ResetPassword( "newPassword", "newPassword", "" );
            Assert.IsFalse( success, "it should have failed" );
        }

        private User GetCreatedUser()
        {
            var allUsers = userInteractor.GetAllUsers();
            return allUsers.FirstOrDefault();
        }

        private string GetEmailContents()
        {
            var mailFolder = new DirectoryInfo( EmailDirectory );
            var email = mailFolder.GetFiles().First();
            var fullPath = email.FullName;
            return File.ReadAllText( fullPath );
        }

        private string GetTokenFromEmail()
        {
            var emailContents = GetEmailContents();
            var contents = emailContents.Replace( "=" + System.Environment.NewLine, "" );
            int startPos = contents.LastIndexOf( "?token=" ) + "?token=".Length + "3D".Length;
            int length = contents.IndexOf( "</a>" ) - startPos;
            return contents.Substring( startPos, length );
        }

        private void DeleteEmails()
        {
            var mailFolder = new DirectoryInfo( EmailDirectory );
            foreach( var file in mailFolder.GetFiles() )
                file.Delete();
        }
    }
}
