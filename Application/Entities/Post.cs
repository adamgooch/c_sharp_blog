using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities
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
