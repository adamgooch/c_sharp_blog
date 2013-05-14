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
        void SetRootDirectory( string directory );
        void CreatePost( Post post );
        IEnumerable<Post> GetAllPosts( string author );
    }
}
