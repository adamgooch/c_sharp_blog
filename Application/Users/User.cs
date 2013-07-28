using System;

namespace Application.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] Salt { get; set; }
        public byte[] PasswordDigest { get; set; }
        public DateTime CreatedDate { get; set; }
        public Role Role { get; set; }
        public Guid VerifiedToken { get; set; }
    }
}
