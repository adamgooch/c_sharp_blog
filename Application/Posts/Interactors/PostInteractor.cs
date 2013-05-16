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

        public void DeletePost( string author, DateTime date, string title )
        {
            postRepository.DeletePost( author, date, title );
        }

        public IEnumerable<Post> GetAllPosts( string author )
        {
            return postRepository.GetAllPosts( author );
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return postRepository.GetAllPosts();
        }

        public Post GetLatestPost()
        {
            var allPosts = postRepository.GetAllPosts();
            var latestPostDate = allPosts.Max( x => x.date );
            return allPosts.First( x => x.date == latestPostDate );
        }
    }
}
