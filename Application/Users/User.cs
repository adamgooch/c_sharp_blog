using System;
using System.Collections.Generic;

namespace Application.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] Salt { get; set; }
        public byte[] PasswordDigest { get; set; }
        public bool Active { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
