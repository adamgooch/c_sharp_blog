using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Register",
                url: "register",
                defaults: new { controller = "Account", action = "Register" }
            );

            routes.MapRoute(
                name: "ShowPost",
                url: "Blog/{author}/{blogTitle}",
                defaults: new { controller = "Blog", action = "ShowPost" }
            );

            routes.MapRoute(
                name: "ShowAuthorPosts",
                url: "Blogs/{author}",
                defaults: new { controller = "Blog", action = "ShowAuthorPosts" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}