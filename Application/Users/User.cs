using System;

namespace Application.Users
{
    public class User
    {
        public string Email { get; set; }
        public byte[] Salt { get; set; }
        public byte[] PasswordDigest { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Role { get; set; }
        public Guid VerifiedToken { get; set; }
    }
}
