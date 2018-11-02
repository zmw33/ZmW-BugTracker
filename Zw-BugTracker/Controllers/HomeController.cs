using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}