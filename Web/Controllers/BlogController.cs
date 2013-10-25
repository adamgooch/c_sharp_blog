using System;
using System.Web.Mvc;
using Application;
using Web.Filters;
using Web.Models.PageModels;
using Application.Posts.Interactors;
using Application.Posts.Entities;
using Data.Repositories;

namespace Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPostInteractor postInteractor;

        public BlogController()
        {
            var postRepository = new FlatFilePostRepository();
            postInteractor = new PostInteractor( postRepository );
        }

        [RoleAuthorization( Roles = new[] { "Author" } )]
        public ActionResult New()
        {
            var newPostPage = new NewPostPage();
            return View( newPostPage );
        }

        [RoleAuthorization( Roles = new[] { "Author" } )]
        [ValidateInput( false )]
        public void Create( Post post )
        {
            if( ModelState.IsValid )
            {
                postInteractor.CreatePost( post );
                Response.Redirect( "Manage" );
            }
            else
            {
                Response.Redirect( "/" );
            }
        }

        public ActionResult ShowPost( string author, string blogTitle )
        {
            var showPostPage = new ShowPostPage( postInteractor, author, blogTitle );
            return View( "Show", showPostPage );
        }

        [RoleAuthorization( Roles = new[] { "Author" } )]
        public void DeletePost( string author, string blogTitle, DateTime date )
        {
            postInteractor.DeletePost( author, date, blogTitle );
            Response.Redirect( "Manage" );
        }

        [RoleAuthorization( Roles = new[] { "Author" } )]
        public ActionResult Manage()
        {
            var manageBlogPage = new ManageBlogPage( postInteractor );
            return View( "Manage", manageBlogPage );
        }

        public ActionResult EditPost( string author, string blogTitle )
        {
            var editPostPage = new EditPostPage( postInteractor, author, blogTitle );
            return View( "Edit", editPostPage );
        }

        [RoleAuthorization( Roles = new[] { "Author" } )]
        [HttpPost]
        [ValidateInput( false )]
        public void Edit( Post post )
        {
            postInteractor.EditPost( post );
            Response.Redirect( "Manage" );
        }
    }
}