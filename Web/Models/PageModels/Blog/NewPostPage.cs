using System;

namespace Web.Models.PageModels
{
    public class NewPostPage
    {
        public string PageHeader = "New Post";
        public NewPostPageForm Post;

        public NewPostPage()
        {
            Post = new NewPostPageForm();
        }
    }

    public class NewPostPageForm
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public string Author = "Adam Gooch";
        public DateTime Date = DateTime.Now;
    }
}