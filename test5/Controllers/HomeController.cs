using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test5.Models;

namespace test5.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        
        public ActionResult Index()
        {
            string User_name = "";
           
          
            User_name = Request.Cookies["Mycookie"].Value;
           
            ViewBag.Message = User_name;

            return View();
        }



       



    }
}