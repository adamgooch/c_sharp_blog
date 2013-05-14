using Application.Interactors;
using Application.Entities;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            NameValueCollection formValues = Request.Form;
            var post = new Post();
            post.author = formValues["Form.Author"];
            post.title = formValues["Form.Title"];
            post.tags = new string[]{ formValues["Form.Tags"] };
            post.body = formValues["Form.Body"];
            postInteractor.CreatePost( post );
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