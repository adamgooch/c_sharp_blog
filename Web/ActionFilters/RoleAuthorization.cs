using System.Web.Mvc;
using System.Linq;
using Application;
using Application.Users;
using Data.Repositories;

namespace Web.Filters
{
    public class RoleAuthorization : ActionFilterAttribute, IAuthorizationFilter
    {
        public string[] Roles { get; set; }

        public void OnAuthorization( AuthorizationContext filterContext )
        {
            var cookie = filterContext.HttpContext.Request.Cookies[Authenticator.AuthenticationCookie];
            if( cookie != null )
            {
                var mailer = new Mailer();
                var sqlRepository = new SqlServerUserRepository();
                var authenticator = new Authenticator( sqlRepository, mailer );
                var user = authenticator.GetUser( cookie );
                if( !Authorized( user ) )
                    filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        private bool Authorized( User user )
        {
            if( user == null ) return false;
            foreach( var role in user.Roles )
            {
                if( Roles.Contains( role ) ) return true;
            }
            return false;
        }
    }
}