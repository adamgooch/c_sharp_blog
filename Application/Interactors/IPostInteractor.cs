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
        void CreatePost( Post post );
        IEnumerable<Post> GetAllPosts( string author );
    }
}
