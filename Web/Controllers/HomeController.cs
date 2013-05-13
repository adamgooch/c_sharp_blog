using Application.Interactors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.PageModels;
using Data.Repositories;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private IPostInteractor postInteractor;

        public HomeController()
        {
            var postRepository = new PostRepository();
            postInteractor = new PostInteractor(postRepository);
        }
        
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult About()
        {
            var model = new AboutPage
            {
                PageTitle = "Yo Mamma"
            };
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult About(AboutPage page)
        {
            page.PageTitle = "Yo Daddy";
            return View(page);
        }

        public ActionResult Rawr()
        {
            var model = new AboutPage
            {
                PageTitle = "Rawr"
            };
            return View("About", model);
        }

        public ActionResult Post(int id)
        {
            //var response = postInteractor.GetPost(id);
            // create view page from response - pass back the view page
            //var model = mapper.toModel(response);
            //return View(model);
            return View("About");
        }
    }
}
