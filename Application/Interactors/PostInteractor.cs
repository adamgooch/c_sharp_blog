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
        private readonly string rootDirectory = "C:\\Users\\Adam\\Documents\\Visual Studio 2012\\Projects\\Blog\\posts";

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
