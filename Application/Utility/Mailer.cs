using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Application
{
    public class Mailer : IMailer
    {
        private static readonly string hostName = ConfigurationManager.AppSettings["HostName"];

        public bool SendVerificationEmail( string email, Guid verificationToken )
        {
            try
            {
                var verificationUrl = hostName + "/account/activate?token=" + verificationToken;
                var mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress( "adamgooch@outlook.com" );
                mailMessage.To.Add( new MailAddress( email ) );
                mailMessage.Subject = "New User Registration";
                mailMessage.Body = "Your registration is almost complete. Please go to the following URL to activate your membership.<br /><br /><a href=\"" +
                  verificationUrl + "\">" + verificationUrl + "</a>" + "<br /><br />Thanks for signing up!";
                var smtpClient = new SmtpClient();
                smtpClient.Send( mailMessage );
                return true;
            }
            catch( Exception e )
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise( e );
                return false;
            }
        }

        public bool SendPasswordResetEmail( string email, string resetToken )
        {
            try
            {
                var passwordResetUrl = hostName + "/account/ResetPassword?token=" + resetToken;
                var mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress( "adamgooch@outlook.com" );
                mailMessage.To.Add( new MailAddress( email ) );
                mailMessage.Subject = "Password Reset";
                mailMessage.Body = "To reset your password, please go to the following URL.<br /><br /><a href=\"" +
                    passwordResetUrl + "\">" + passwordResetUrl + "</a>";
                var smtpClient = new SmtpClient();
                smtpClient.Send( mailMessage );
                return true;
            }
            catch( Exception e )
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise( e );
                return false;
            }
        }
    }
}
