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
                Response.Redirect( "/" );
            }
            else
            {
                Response.Redirect( "/" );
            }
        }

        public ActionResult ShowPost()
        {
            var author = RouteData.Values["author"];
            var title = RouteData.Values["blogTitle"];
            @ViewBag.Title = title;
            var showPostPage = new ShowPostPage( postInteractor, author.ToString(), title.ToString() );
            return View( "Show", showPostPage );
        }

        public void DeletePost()
        {
            var author = Request.QueryString["author"];
            var title = Request.QueryString["blogTitle"];
            var date = DateTime.Parse( Request.QueryString["date"] );
            postInteractor.DeletePost(
                author.ToString(),
                date, 
                title.ToString()
            );
            Response.Redirect( "Manage" );
        }

        public ActionResult Manage()
        {
            if( LoggedIn() )
            {
                var manageBlogPage = new ManageBlogPage( postInteractor );
                manageBlogPage.PageTitle = "Manage";
                return View( "Manage", manageBlogPage );
            }
            else
            {
                var loginPage = new LoginPage { ReturnUrl = "Blog/New" };
                return View( "Login", loginPage );
            }
        }

        public ActionResult EditPost()
        {
            if( LoggedIn() )
            {
                var author = Request.QueryString["author"];
                var title = Request.QueryString["blogTitle"];
                var editPostPage = new EditPostPage( postInteractor, author.ToString(), title.ToString() );
                editPostPage.PageTitle = "New Post";
                return View( "Edit", editPostPage );
            }
            else
            {
                var loginPage = new LoginPage { ReturnUrl = "Blog/New" };
                return View( "Login", loginPage );
            }
        }

        [HttpPost]
        public void Edit()
        {
            if( LoggedIn() )
            {
                NameValueCollection formValues = Request.Unvalidated.Form;
                var post = postInteractor.GetPost( "Adam Gooch", formValues["Form.Title"] );
                post.Title = formValues["Form.Title"];
                post.Tags = new string[] { formValues["Form.Tags"] };
                post.Body = formValues["Form.Body"];
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
            return true;
            return (string)Session["id_1"] == ConfigurationManager.AppSettings["SessionValue1"] &&
                (string)Session["id_2"] == ConfigurationManager.AppSettings["SessionValue2"] &&
                (string)Session["id_3"] == ConfigurationManager.AppSettings["SessionValue3"];
        }
    }
}