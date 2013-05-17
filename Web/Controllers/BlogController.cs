﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            var postRepository = new PostRepository();
            postInteractor = new PostInteractor( postRepository );
        }

        public ActionResult New()
        {
            if( LoggedIn() )
            {
                @ViewBag.Title = "New Post";
                var newPostPage = new NewPostPage();
                newPostPage.PageTitle = "New Post";
                return View( "New", newPostPage );
            }
            else
            {
                var loginPage = new LoginPage { ReturnUrl = "Blog/New" };
                return View( "Login", loginPage );
            }
        }

        public void Create()
        {
            NameValueCollection formValues = Request.Unvalidated.Form;
            var post = new Post();
            post.author = formValues["Form.Author"];
            post.title = formValues["Form.Title"];
            post.tags = new string[]{ formValues["Form.Tags"] };
            post.body = formValues["Form.Body"];
            postInteractor.CreatePost( post );
            Response.Redirect( "Index" );
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
            @ViewBag.Title = "Manage Blog";
            var manageBlogPage = new ManageBlogPage( postInteractor );
            manageBlogPage.PageTitle = "Manage";
            return View( "Manage", manageBlogPage );
        }

        public ActionResult EditPost()
        {
            @ViewBag.Title = "Edit Post";
            var author = Request.QueryString["author"];
            var title = Request.QueryString["blogTitle"];
            var editPostPage = new EditPostPage( postInteractor, author.ToString(), title.ToString() );
            editPostPage.PageTitle = "New Post";
            return View( "Edit", editPostPage );
        }

        [HttpPost]
        public void Edit()
        {
            NameValueCollection formValues = Request.Unvalidated.Form;
            var post = postInteractor.GetPost( formValues["Form.Author"], formValues["Form.Title"] );
            post.title = formValues["Form.Title"];
            post.tags = new string[] { formValues["Form.Tags"] };
            post.body = formValues["Form.Body"];
            postInteractor.EditPost( post );
            Response.Redirect( "Index" );
        }

        private bool LoggedIn()
        {
            return Session["id_1"] == System.Configuration.ConfigurationManager.AppSettings["SessionValue1"] &&
                Session["id_2"] == System.Configuration.ConfigurationManager.AppSettings["SessionValue2"] &&
                Session["id_3"] == System.Configuration.ConfigurationManager.AppSettings["SessionValue3"];
        }
    }
}