using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.PageModels;
using Data.Repositories;
using Application.Posts.Interactors;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private IPostInteractor postInteractor;

        public HomeController()
        {
            var postRepository = new PostRepository();
            postInteractor = new PostInteractor( postRepository );
        }
        
        public ActionResult Index()
        {
            var homePage = new HomePage( postInteractor );
            homePage.PageTitle = "Adam Gooch";
            return View( "Index", homePage );
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
