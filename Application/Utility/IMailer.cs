using System;

namespace Application
{
    public interface IMailer
    {
        bool SendVerificationEmail( string email, Guid verificationToken );
        bool SendPasswordResetEmail( string email, string resetToken );
    }
}
