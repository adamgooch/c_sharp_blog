using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Application
{
    public class Mailer : IMailer
    {
        private readonly SmtpClient smtp;
        private readonly MailAddress fromAddress;
        private readonly string rootDirectory = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "keys";
        private readonly string emailFile;
        private string fromEmail;
        private string fromEmailPassword;

        public Mailer()
        {
            emailFile = String.Format( "{0}\\emailSecret", rootDirectory );
            GetFromAddressAndPassword();
            fromAddress = new MailAddress( fromEmail, "AdamGooch.me" );
            smtp = new SmtpClient
            {
                Host = "smtp.live.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential( fromAddress.Address, fromEmailPassword )
            };
        }

        public void SendNewUserVerificationEmail( string email, Guid verificationToken )
        {
            var toAddress = new MailAddress( email, email );
            const string subject = "AdamGooch.me New User Verification";
            var body = String.Format( "Follow this link to complete your registration: http://localhost:50508/verify_user/{0}", verificationToken );

            using( var message = new MailMessage( fromAddress, toAddress ) )
            {
                message.Subject = subject;
                message.Body = body;
                smtp.Send( message );
            }
        }

        private void GetFromAddressAndPassword()
        {
            if( !Directory.Exists( rootDirectory ) ) Directory.CreateDirectory( rootDirectory );
            if( File.Exists( emailFile ) )
            {
                var emailValues = File.ReadAllText( emailFile ).Split( ',' );
                fromEmail = emailValues[0];
                fromEmailPassword = emailValues[1];
            }
            else
            {
                fromEmail = "example@email";
                fromEmailPassword = "password";
                File.WriteAllText( emailFile, String.Format( "{0},{1}", fromEmail, fromEmailPassword ) );
            }
        }
    }
}
