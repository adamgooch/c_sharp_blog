using System.Web.Mvc;
using Application;
using Application.Users;
using Data.Repositories;

namespace Web.Filters
{
    public class AuthorizeUser : AuthorizeAttribute
    {
        public Role Role { get; set; }

        public override void OnAuthorization( AuthorizationContext filterContext )
        {
            var request = filterContext.HttpContext.Request;
            var cookie = request.Cookies[Authenticator.AuthenticationCookie];
            if( cookie != null )
            {
                var userInteractor = new UserInteractor( new SQLServerUserRepository(), new Authenticator(), new Mailer() );
                var user = userInteractor.GetUserByCookie( cookie );
                if( user != null && user.Role >= Role ) SetCurrentUser( filterContext, user );
                else RedirectToLogin( filterContext );
            }
            else
            {
                RedirectToLogin( filterContext );
            }
        }

        private void RedirectToLogin( AuthorizationContext filterContext )
        {
            filterContext.Result = new RedirectResult( "/account/login" );
        }

        private void SetCurrentUser( AuthorizationContext filterContext, User user )
        {
            filterContext.Controller.ViewBag.Username = user.Email;
        }
    }
}
