using System.Collections.Generic;
using System.Linq;
using Application.Posts.Entities;
using Application.Posts.Interactors;

namespace Web.Models.PageModels
{
    public class ShowPostPage
    {
        public Post post { get; set; }
        public IOrderedEnumerable<Post> AllPosts { get; set; }

        public ShowPostPage( IPostInteractor postInteractor, string author, string title )
        {
            var posts = (List<Post>)postInteractor.GetAllPosts( author );
            post = posts.Find( p => p.Title == title.Replace('_', ' ' ) );
            AllPosts = postInteractor.GetAllPosts().OrderByDescending( x => x.Date );
        }
    }
}