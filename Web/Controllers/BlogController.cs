using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Web.Models.PageModels;
using Application.Posts.Interactors;
using Application.Posts.Entities;
using Data.Repositories;

namespace Web.Controllers
{
    public class BlogController : Controller
    {
        private IPostInteractor postInteractor;

        public BlogController()
        {
            var postRepository = new FlatFilePostRepository();
            postInteractor = new PostInteractor( postRepository );
        }

        public ActionResult New()
        {
            if( LoggedIn() )
            {
                var newPostPage = new NewPostPage();
                return View( newPostPage );
            }
            else
            {
                var loginPage = new LoginPage { ReturnUrl = "Blog/New" };
                return View( "Login", loginPage );
            }
        }

        [ValidateInput(false)]
        public void Create(Post post)
        {
            if( LoggedIn() && ModelState.IsValid )
            {
                postInteractor.CreatePost( post );
                Response.Redirect( "Manage" );
            }
            else
            {
                Response.Redirect( "/" );
            }
        }

        public ActionResult ShowPost(string author, string blogTitle)
        {
            var showPostPage = new ShowPostPage( postInteractor, author, blogTitle );
            return View( "Show", showPostPage );
        }

        public void DeletePost(string author, string blogTitle, DateTime date)
        {
            postInteractor.DeletePost( author, date, blogTitle );
            Response.Redirect( "Manage" );
        }

        public ActionResult Manage()
        {
            if( LoggedIn() )
            {
                var manageBlogPage = new ManageBlogPage( postInteractor );
                return View( "Manage", manageBlogPage );
            }
            else
            {
                var loginPage = new LoginPage { ReturnUrl = "Blog/New" };
                return View( "Login", loginPage );
            }
        }

        public ActionResult EditPost(string author, string blogTitle)
        {
            if( LoggedIn() )
            {
                var editPostPage = new EditPostPage( postInteractor, author, blogTitle );
                return View( "Edit", editPostPage );
            }
            else
            {
                var loginPage = new LoginPage { ReturnUrl = "Blog/New" };
                return View( "Login", loginPage );
            }
        }

        [HttpPost]
        [ValidateInput( false )]
        public void Edit( Post post )
        {
            if( LoggedIn() )
            {
                postInteractor.EditPost( post );
                Response.Redirect( "Manage" );
            }
            else
            {
                Response.Redirect( "/" );
            }
        }

        private bool LoggedIn()
        {
            return (string)Session["id_1"] == ConfigurationManager.AppSettings["SessionValue1"] &&
                (string)Session["id_2"] == ConfigurationManager.AppSettings["SessionValue2"] &&
                (string)Session["id_3"] == ConfigurationManager.AppSettings["SessionValue3"];
        }
    }
}