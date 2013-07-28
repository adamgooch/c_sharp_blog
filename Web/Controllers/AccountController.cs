using System;
using Application;
using Application.Users;
using Data.Repositories;
using System.Collections.Specialized;
using System.Web.Mvc;
using Web.Models.PageModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticator authenticator;
        private IUserInteractor userInteractor;

        public AccountController()
        {
            authenticator = new Authenticator();
            var userRepository = new SQLServerUserRepository();
            userInteractor = new UserInteractor( userRepository, authenticator );
        }

        [HttpGet]
        public ActionResult Register()
        {
            var registerPage = new RegisterPage();
            return View( registerPage );
        }

        public ActionResult VerifyUser( Guid token )
        {
            userInteractor.VerifyUser( token );
            return RedirectToAction( "Index", "Home" );
        }

        [HttpPost]
        public ActionResult Register( RegisterPage model )
        {
            NameValueCollection formValues = Request.Form;
            if( !ModelState.IsValid ) return View( model );
            userInteractor.CreateUser( formValues["email"], formValues["password"] );
            return RedirectToAction( "Index", "Home" );
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login( LoginPage model, string ReturnUrl )
        {
            NameValueCollection formValues = Request.Form;
            var user = userInteractor.GetUserByUsername( formValues["username"] );
            if( authenticator.Authenticate( formValues["password"], user.Salt, user.PasswordDigest, 5000 ) )
            {
                Response.Cookies.Add( authenticator.GenerateAuthenticationCookie( user.Id, user.Salt ) );
                return RedirectToAction( "Index", "Home" );
            }
            else
                return View( model );
        }

        public ActionResult Manage()
        {
            var pageModel = new ManageAccountsPage( userInteractor );
            return View( pageModel );
        }

        [HttpPost]
        public void Delete()
        {
            Response.Redirect( "Manage" );
        }
    }
}
