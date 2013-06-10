using System;

namespace Application.Posts.Entities
{
    public class Post
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string[] Tags { get; set; }
    }
}
