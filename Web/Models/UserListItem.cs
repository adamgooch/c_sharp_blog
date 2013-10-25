using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class UserListItem
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public IEnumerable<string> Roles { get; set; }

        private IEnumerable<SelectListItem> rolesList;

        public IEnumerable<SelectListItem> RolesList
        {
            get
            {
                return from role in Roles
                       select new SelectListItem
                       {
                           Selected = ( role == Roles.FirstOrDefault() ),
                           Text = role,
                           Value = role
                       };
            }
            set
            {
                rolesList = value;
            }
        }
    }
}