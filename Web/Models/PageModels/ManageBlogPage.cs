using Application.Posts.Entities;
using Application.Posts.Interactors;
using System.Collections.Generic;

namespace Web.Models.PageModels
{
    public class ManageBlogPage
    {
        public List<Post> AllPosts { get; set; }
        public string PageHeader = "Manage";

        public ManageBlogPage( IPostInteractor postInteractor )
        {
            AllPosts = (List<Post>)postInteractor.GetAllPosts();
        }
    }
}