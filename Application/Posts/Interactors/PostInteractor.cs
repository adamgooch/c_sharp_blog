using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application.Posts.Entities;
using Application.Posts.RepositoryContracts;

namespace Application.Posts.Interactors
{
    public class PostInteractor : IPostInteractor
    {
        private readonly IPostRepository postRepository;
        private readonly string rootDirectory = "C:\\Users\\Adam\\Documents\\Visual Studio 2012\\Projects\\Blog\\site_posts";

        public PostInteractor(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
            postRepository.SetRootDirectory( rootDirectory );
        }

        public void CreatePost( Post post )
        {
            post.date = DateTime.Now;
            postRepository.CreatePost( post );
        }

        public IEnumerable<Post> GetAllPosts( string author )
        {
            return postRepository.GetAllPosts( author );
        }
    }
}
