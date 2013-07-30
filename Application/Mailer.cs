using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Application
{
    public class Mailer : IMailer
    {
        private readonly SmtpClient smtp;
        private readonly MailAddress fromAddress;
        private readonly string password;

        public Mailer()
        {
            fromAddress = new MailAddress( ConfigurationManager.AppSettings["EmailFromAddress"], "AdamGooch.me" );
            password = ConfigurationManager.AppSettings["EmailPassword"];
            smtp = new SmtpClient
            {
                Host = "smtp.live.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential( fromAddress.Address, password )
            };
        }

        public void SendNewUserVerificationEmail( string email, Guid verificationToken )
        {
            var toAddress = new MailAddress( email, email );
            const string subject = "AdamGooch.me New User Verification";
            var body =
                String.Format( "Follow this link to complete your registration: http://localhost:50508/verify_user/{0}",
                              verificationToken );
            
            using( var message = new MailMessage( fromAddress, toAddress )
                {
                    Subject = subject,
                    Body = body
                } )
            {
                smtp.Send( message );
            }
        }
    }
}
