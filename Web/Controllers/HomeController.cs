using System;
using System.Web.Mvc;
using Application;
using Application.Users;
using Web.Models.PageModels;
using Data.Repositories;
using Application.Posts.Interactors;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostInteractor postInteractor;

        public HomeController()
        {
            var postRepository = new FlatFilePostRepository();
            postInteractor = new PostInteractor( postRepository );
        }

        public ActionResult Index()
        {
            var homePage = new HomePage( postInteractor );
            homePage.PageTitle = "Adam Gooch";
            var authenticator = new Authenticator();
            var cookie = Request.Cookies[Authenticator.AuthenticationCookie];
            if (cookie != null)
            {
                var decryptedCookie = authenticator.DecryptAuthenticationCookie( cookie );
                var userId = decryptedCookie.Values["Id"];
                var userInteractor = new UserInteractor( new SqlServerUserRepository(), authenticator, new Mailer() );
                var user = userInteractor.GetUserById( new Guid( userId ) );
                ViewBag.Username = user.Email;
            
            }
            return View( "Index", homePage );
        }
    }
}
