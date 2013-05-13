using Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositoryContracts
{
    public interface IPostRepository
    {
        void CreatePost(string title, string body, DateTime date, string author);
        IEnumerable<Post> GetAllPosts();
    }
}
