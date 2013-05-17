using Application.Posts.Entities;
using Application.Posts.Interactors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.PageModels
{
    public class HomePage
    {
        public string PageTitle { get; set; }
        public Post LatestPost { get; set;  }
        public IOrderedEnumerable<Post> AllPosts { get; set; }

        public HomePage( IPostInteractor postInteractor )
        {
            LatestPost = postInteractor.GetLatestPost();
            AllPosts = postInteractor.GetAllPosts().OrderByDescending( x => x.date );
        }
    }
}