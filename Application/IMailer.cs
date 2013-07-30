using System;

namespace Application
{
    public interface IMailer
    {
        void SendNewUserVerificationEmail( string email, Guid verificationToken );
    }
}
