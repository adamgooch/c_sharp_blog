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
            return View( "Index", homePage );
        }

        public ActionResult Failure()
        {
            return View();
        }
    }
}
