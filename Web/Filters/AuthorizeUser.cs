using System.Web.Mvc;
using Application;
using Application.Users;
using Data.Repositories;

namespace Web.Filters
{
    public class AuthorizeUser : AuthorizeAttribute
    {
        public override void OnAuthorization( AuthorizationContext filterContext )
        {
            var request = filterContext.HttpContext.Request;
            var cookie = request.Cookies[Authenticator.AuthenticationCookie];
            var authenticator = new Authenticator();
            if( cookie != null )
            {
                var userInteractor = new UserInteractor( new SQLServerUserRepository(), authenticator );
                var user = userInteractor.GetUserByCookie( cookie );
                if( user != null ) SetCurrentUser( filterContext, user );
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
