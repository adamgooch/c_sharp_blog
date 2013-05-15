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
        
        public HomeController()
        {
            
        }
        
        public ActionResult Index()
        {
            return View();
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
