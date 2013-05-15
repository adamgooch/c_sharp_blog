using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Application.Posts.Interactors;
using Application.Posts.Entities;

namespace Web.Models.PageModels
{
    public class PostIndexPage
    {
        public string PageTitle { get; set; }
        public List<Post> Posts { get; set; }

        public PostIndexPage( IPostInteractor postInteractor )
        {
            Posts = (List<Post>)postInteractor.GetAllPosts( "Adam Gooch" );
        }

        public PostIndexPage( IPostInteractor postInteractor, string author )
        {
            Posts = (List<Post>)postInteractor.GetAllPosts( author );
        }

        public string FormatDate( DateTime date )
        {
            return date.ToString( "MMMM dd, yyyy" );
        }
    }
}