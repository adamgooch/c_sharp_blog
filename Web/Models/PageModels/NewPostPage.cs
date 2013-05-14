using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.PageModels
{
    public class NewPostPage
    {
        public string PageTitle { get; set; }
        public NewPostPageForm Form { get; set; }

        public NewPostPage()
        {
            Form = new NewPostPageForm();
        }
    }

    public class NewPostPageForm
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
    }
}