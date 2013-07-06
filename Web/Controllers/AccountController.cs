using SimpleCrypto;
using System.Collections.Specialized;
using System.Web.Mvc;
using Web.Models.PageModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            var registerPage = new RegisterPage();
            return View( registerPage );
        }

        [HttpPost]
        public ActionResult Register(RegisterPage model)
        {
            NameValueCollection formValues = Request.Form;
            if( !ModelState.IsValid )
            {
                return View( model );
            }
            return RedirectToAction( "Index", "Home" );
        }
        
        [ValidateAntiForgeryToken]
        public ActionResult Login( LoginPage model, string ReturnUrl )
        {
            NameValueCollection formValues = Request.Form;
            if( Validated( formValues["username"], formValues["password"] ) )
            {
                Session["id_1"] = System.Configuration.ConfigurationManager.AppSettings["SessionValue1"];
                Session["id_2"] = System.Configuration.ConfigurationManager.AppSettings["SessionValue2"];
                Session["id_3"] = System.Configuration.ConfigurationManager.AppSettings["SessionValue3"];
                return RedirectToLocal( model.ReturnUrl );
            }
            else
                return View( model );
        }

        private ActionResult RedirectToLocal( string returnUrl )
        {
            if( Url.IsLocalUrl( returnUrl ) )
            {
                return Redirect( returnUrl );
            }
            return RedirectToAction( "Manage", "Blog" );
        }

        private bool Validated( string username, string password )
        {
            var crypto = new PBKDF2();
            var hashedPass = crypto.Compute( password, System.Configuration.ConfigurationManager.AppSettings["ManageSalt"] );
            return username == System.Configuration.ConfigurationManager.AppSettings["ManageUser"] &&
                 hashedPass == System.Configuration.ConfigurationManager.AppSettings["ManagePass"];
        }
    }
}
