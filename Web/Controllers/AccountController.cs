using System;
using Application;
using Application.Users;
using Data.Repositories;
using System.Collections.Specialized;
using System.Web.Mvc;
using Web.Filters;
using Web.Models.PageModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticator authenticator;
        private readonly IUserInteractor userInteractor;

        public AccountController()
        {
            authenticator = new Authenticator();
            userInteractor = new UserInteractor( new SQLServerUserRepository(), authenticator, new Mailer() );
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
            if( authenticator.Verified(user.VerifiedToken) && authenticator.Authenticate( formValues["password"], user.Salt, user.PasswordDigest, 5000 ) )
            {
                Response.Cookies.Add( authenticator.GenerateAuthenticationCookie( user.Id, user.Salt ) );
                return RedirectToAction( "Index", "Home" );
            }
            else
                return View( model );
        }

        [AuthorizeUser( Role = Role.Administrator )]
        public ActionResult Manage()
        {
            var pageModel = new ManageAccountsPage( userInteractor );
            return View( pageModel );
        }

        [HttpGet]
        [AuthorizeUser( Role = Role.Administrator )]
        public void Delete( string id )
        {
            var userId = new Guid( id );
            if( !authenticator.LoggedIn( userId, Request.Cookies[Authenticator.AuthenticationCookie] ) )
                userInteractor.DeleteById( userId );
            Response.Redirect( "/account/manage" );
        }

        [HttpPost]
        public void EditRole( Guid id, Role role )
        {
            userInteractor.EditRole( id, role );
        }
    }
}
