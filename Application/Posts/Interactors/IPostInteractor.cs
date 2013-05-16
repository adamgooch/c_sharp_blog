using System;
using System.Collections.Generic;
using Application.Posts.Entities;

namespace Application.Posts.Interactors
{
    public interface IPostInteractor
    {
        void CreatePost( Post post );
        void DeletePost( string author, DateTime date, string title );
        void EditPost( Post post );
        IEnumerable<Post> GetAllPosts( string author );
        IEnumerable<Post> GetAllPosts();
        Post GetLatestPost();
        Post GetPost( string author, string title );
    }
}
