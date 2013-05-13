using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.PageModels
{
    public class AboutPage
    {
        public string PageTitle { get; set; }

        public AboutPage()
        {
            Form = new AboutPageForm();
        }

        public AboutPageForm Form { get; set; }
    }

    public class AboutPageForm
    {
        public string Note { get; set; }
    }
}