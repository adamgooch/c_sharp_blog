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
        
        public PostInteractor(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
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

        public void EditPost( Post post )
        {
            postRepository.CreatePost( post );
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

        public Post GetPost( string author, string title )
        {
            var authorPosts = GetAllPosts( author );
            return authorPosts.First( x => x.title == title );
        }
    }
}
