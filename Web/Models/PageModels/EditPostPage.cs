using Application.Posts.Entities;
using Application.Posts.Interactors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.PageModels
{
    public class EditPostPage
    {
        public string PageTitle { get; set; }
        public EditPostPageForm Form { get; set; }

        public EditPostPage( IPostInteractor postInteractor, string author, string title )
        {
            Form = new EditPostPageForm();
            var posts = (List<Post>)postInteractor.GetAllPosts( author );
            var post = posts.Find( p => p.title == title.Replace( '_', ' ' ) );
            Form.Author = post.author;
            Form.Title = post.title;
            Form.Body = post.body;
            Form.Tags = ( ParseTags( post.tags ) );
        }

        private string ParseTags( string[] tags )
        {
            var result = tags.First();
            if( tags.Count() > 1 )
                foreach( var tag in tags )
                {
                    result = string.Format( "{0}, {1}", result, tag );
                }
            return result;
        }
    }

    public class EditPostPageForm
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
    }
}