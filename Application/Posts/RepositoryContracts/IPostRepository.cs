using System;
using System.Collections.Generic;
using Application.Posts.Entities;

namespace Application.Posts.RepositoryContracts
{
    public interface IPostRepository
    {
        void SetRootDirectory( string directory );
        void CreatePost( Post post );
        IEnumerable<Post> GetAllPosts( string author );
        IEnumerable<Post> GetAllPosts();
        void DeletePost( string author, DateTime date, string title );
    }
}
