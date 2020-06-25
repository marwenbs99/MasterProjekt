using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test5.Models;

namespace test5.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Create()
        {
            var lista = new List<String>();
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Select(a => a.Email).ToList();
                for (int i = 0; i== (v.Count)-1; i++)
                {
                    lista.Add(v[i]);
                }
                return View(v);
            }
           


    }
        [HttpPost]
        public ActionResult Create(Projects p, FormCollection collection)
        {
            List<string> usersToAdd = new List<string>();
            
            string mail = Request.Cookies["Mycookie"].Value;
            Projects p1 = new Projects();
            p1.Name = p.Name;
            p1.Description = p.Description;
            p1.Statut = "Not defined";
            
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == mail).FirstOrDefault();
                p1.UserID = v.UserID;
                dc.Projects.Add(p1);
                dc.Configuration.ValidateOnSaveEnabled = false;
                dc.SaveChanges();
            }
            return RedirectToAction("index", "Home");
        }

        [Authorize]
        public ActionResult Projectdetails(int? ID)
        {
            String mail = Request.Cookies["Mycookie"].Value;
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == mail).FirstOrDefault();
                ViewBag.name = v.FirstName;
                ViewBag.img = v.ImageUrl;
            }
            return View();
        }

        public ActionResult AddPartner(int ID )
        {
            return View();
        }

    }
}