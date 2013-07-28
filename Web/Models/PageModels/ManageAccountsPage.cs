using System.Collections.Generic;
using System.Linq;
using Application.Users;

namespace Web.Models.PageModels
{
    public class ManageAccountsPage
    {
        public List<User> AllUsers { get; set; }
        public User CurrentUser { get; set; }
        public string PageHeader = "Manage";

        public ManageAccountsPage( IUserInteractor userInteractor )
        {
            AllUsers = userInteractor.GetAllUsers().ToList();
        }
    }
}