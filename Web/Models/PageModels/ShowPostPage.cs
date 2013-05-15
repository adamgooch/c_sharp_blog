using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Application.Posts.Entities;
using Application.Posts.Interactors;

namespace Web.Models.PageModels
{
    public class ShowPostPage
    {
        public Post post { get; set; }

        public ShowPostPage( IPostInteractor postInteractor, string author, string title )
        {
            var posts = (List<Post>)postInteractor.GetAllPosts( author );
            post = posts.Find( p => p.title == title.Replace('_', ' ' ) );
        }
    }
}