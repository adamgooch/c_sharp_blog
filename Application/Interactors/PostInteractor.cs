using Application.Entities;
using Application.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interactors
{
    public class PostInteractor : IPostInteractor
    {
        private readonly IPostRepository postRepository;

        public PostInteractor(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public void CreatePost(string title, string body, string author)
        {
            postRepository.CreatePost(title, body, DateTime.Now, author);
        }

        public Post FindByTitle(string title)
        {
            return new Post();
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return postRepository.GetAllPosts();
        }
    }
}
