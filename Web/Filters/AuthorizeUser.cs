using System;
using System.Web.Mvc;
using Application;
using Application.Users;
using Data.Repositories;

namespace Web.Filters
{
    public class AuthorizeUser : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var cookie = request.Cookies[Authenticator.AuthenticationCookie];
            if (cookie != null)
            {
                var authenticator = new Authenticator();
                var decryptedCookie = authenticator.DecryptAuthenticationCookie(cookie);
                var userId = decryptedCookie.Values["Id"];
                var userInteractor = new UserInteractor(new SQLServerUserRepository(), authenticator);
                var user = userInteractor.GetUserById(new Guid(userId));
                filterContext.Controller.ViewBag.Username = user.Email;
            }
            else
            {
                filterContext.Result = new RedirectResult("/account/login");
            }
        }
    }
}
