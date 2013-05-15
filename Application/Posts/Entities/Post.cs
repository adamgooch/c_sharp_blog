using System;

namespace Application.Posts.Entities
{
    public class Post
    {
        public string title { get; set; }
        public string body { get; set; }
        public string author { get; set; }
        public DateTime date { get; set; }
        public string[] tags { get; set; }
    }
}
