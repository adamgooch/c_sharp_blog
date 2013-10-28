using System.Collections.Generic;
using System.Linq;
using Application.Users;

namespace Web.Models.PageModels
{
    public class ManageAccountsPage
    {
        public int AllRolesId { get; set; }
        public List<UserListItem> AllUsers { get; set; }
        public List<string> AllRoles { get; set; }
        public string PageHeader = "Manage";

        public ManageAccountsPage( IUserInteractor userInteractor )
        {
            var allUsers = userInteractor.GetAllUsers();
            AllRoles = userInteractor.GetAllRoles().ToList();
            AllUsers = new List<UserListItem>();
            foreach( var user in allUsers )
            {
                var userView = new UserListItem
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.Roles,
                    Active = user.Active
                };
                AllUsers.Add( userView );
            }
        }
    }
}