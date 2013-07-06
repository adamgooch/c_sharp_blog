using System;

namespace Application.Users
{
    public class User
    {
        public string Email { get; set; }
        public string Salt { get; set; }
        public string PasswordDigest { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Role { get; set; }
    }
}
