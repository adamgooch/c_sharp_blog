using Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interactors
{
    public interface IPostInteractor
    {
        void CreatePost(string title, string body, string author);
        Post FindByTitle(string title);
        IEnumerable<Post> GetAllPosts();
    }
}
