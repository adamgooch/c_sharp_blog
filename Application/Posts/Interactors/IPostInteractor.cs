using System;
using System.Collections.Generic;
using Application.Posts.Entities;

namespace Application.Posts.Interactors
{
    public interface IPostInteractor
    {
        void CreatePost( Post post );
        IEnumerable<Post> GetAllPosts( string author );
        IEnumerable<Post> GetAllPosts();
        Post GetLatestPost();
        void DeletePost( string author, DateTime date, string title );
    }
}
