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
        IAuthenticate authenticator;
        IUserInteractor userInteractor;

        public AccountController()
        {
            var mailer = new Mailer();
            var userRepository = new SqlServerUserRepository();
            userInteractor = new UserInteractor( userRepository );
            authenticator = new Authenticator( userRepository, mailer );
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register( string email, string password, string passwordConfirmation, RegisterPage model )
        {
            if( ModelState.IsValid && authenticator.CreateUser( email, password, passwordConfirmation ) )
            {
                return RedirectToAction( "Index", "Home" );
            }
            else
            {
                return View( model );
            }
        }

        [HttpGet]
        public bool UsernameExists( string email )
        {
            return userInteractor.UsernameExists( email );
        }

        [HttpGet]
        public ActionResult Activate( Guid token )
        {
            if( authenticator.ActivateUser( token ) )
            {
                return View();
            }
            else
            {
                return RedirectToAction( "Failure", "Home" );
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login( string email, string password, bool rememberMe )
        {
            if( authenticator.Authenticate( email, password ) )
            {
                Response.Cookies.Add( authenticator.GenerateAuthCookie( email, rememberMe ) );
                return RedirectToAction( "Index", "Home" );
            }
            else
            {
                return RedirectToAction( "Failure", "Home" );
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var cookie = Response.Cookies[Authenticator.AuthenticationCookie];
            if( cookie != null ) cookie.Expires = DateTime.Now.AddDays( -1D );
            return RedirectToAction( "Index", "Home" );
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword( string email )
        {
            /*
             * There is some controversy about password reset security. This email method is prone
             * to man in the middle attackers and inherent email insecurities. Security questions
             * is another method that is prone to targeted attacks. My belief is to let the user
             * choose which vulnerability they would prefer to be susceptible to. Therefore, I will
             * default to email since I have collected that information for login anyway, then allow
             * the user to add security questions if they prefer to reset their password that way.
             * */
            if( authenticator.SendPasswordResetEmail( email ) )
            {
                return RedirectToAction( "Index", "Home" );
            }
            else
            {
                return RedirectToAction( "Failure", "Home" );
            }
        }

        [HttpGet]
        public ActionResult ResetPassword( string token )
        {
            Session["reset_token"] = token;
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword( string password, string passwordConfirmation )
        {
            var token = Session["reset_token"];
            if( token == null )
            {
                return RedirectToAction( "Failure", "Home" );
            }
            else
            {
                if( authenticator.ResetPassword( password, passwordConfirmation, (string)token ) )
                {
                    return RedirectToAction( "Index", "Home" );
                }
                else
                {
                    return RedirectToAction( "Failure", "Home" );
                }
            }
        }

        [RoleAuthorization( Roles = new[] { "Admin" } )]
        [HttpGet]
        public ActionResult ManageAll()
        {
            var model = new ManageAccountsPage( userInteractor );
            return View( model );
        }

        [RoleAuthorization( Roles = new[] { "Admin" } )]
        [HttpPost]
        public bool Delete( string accountId )
        {
            return userInteractor.DeleteUser( new Guid( accountId ) );
        }

        [RoleAuthorization( Roles = new[] { "Admin" } )]
        [HttpPost]
        public bool AddRole( string accountId, string newRole )
        {
            return userInteractor.AddRole( new Guid( accountId ), newRole );
        }

        [RoleAuthorization( Roles = new[] { "Admin" } )]
        [HttpPost]
        public bool RemoveRole( string accountId, string roleToRemove )
        {
            return userInteractor.RemoveRole( new Guid( accountId ), roleToRemove );
        }

        [RoleAuthorization( Roles = new[] { "Admin" } )]
        [HttpPost]
        public bool Activate( string accountId )
        {
            return userInteractor.SetActive( new Guid( accountId ), true );
        }

        [RoleAuthorization( Roles = new[] { "Admin" } )]
        [HttpPost]
        public bool Deactivate( string accountId )
        {
            return userInteractor.SetActive( new Guid( accountId ), false );
        }
    }
}
