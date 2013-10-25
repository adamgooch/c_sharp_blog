using Application;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ActionFilters
{
    public class LoggedOnUser : ActionFilterAttribute
    {
        public override void OnActionExecuting( ActionExecutingContext filterContext )
        {
            var cookie = filterContext.HttpContext.Request.Cookies[Authenticator.AuthenticationCookie];
            if( cookie != null )
            {
                var mailer = new Mailer();
                var sqlRepository = new SqlServerUserRepository();
                var authenticator = new Authenticator( sqlRepository, mailer );
                var user = authenticator.GetUser( cookie );
                var viewBag = filterContext.Controller.ViewBag;
                viewBag.User = user;
            }
        }
    }
}