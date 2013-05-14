using Application.Interactors;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.PageModels;

namespace Web.Controllers
{
    public class BlogController : Controller
    {
        private IPostInteractor postInteractor;

        public BlogController()
        {
            var postRepository = new PostRepository();
            postInteractor = new PostInteractor( postRepository );
        }

        public ActionResult New()
        {
            @ViewBag.Title = "New Post";
            var newPostPage = new NewPostPage();
            newPostPage.PageTitle = "New Post";
            return View( "New", newPostPage );
        }

        public void Create()
        {
            Response.Redirect( "Index" );
        }

        public ActionResult Index()
        {
            @ViewBag.Title = "All Posts";
            var postIndexPage = new PostIndexPage();
            postIndexPage.PageTitle = "All Posts";
            return View( "Index", postIndexPage );
        }
    }
}