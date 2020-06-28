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
            return View(ID);
        }

        [HttpGet]
        public ActionResult AddPartner(int id)
        {
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Select(a => a.Email).ToList();
                var lista = new List<String>();
                lista = v;
               
                ViewBag.Statut = new SelectList(lista);
                MembreElem m = new MembreElem();
                m.projektID = id;
                ViewBag.Log = Emailliste(id);



                return View(m);
            }

        }

        [HttpPost]
        public ActionResult AddPartner(MembreElem e)
        {
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                Memebre m = new Memebre();
                m.UserID = dc.Users.Where(a => a.Email == e.email).FirstOrDefault().UserID;
                m.ProjectID = e.projektID;
                dc.Memebre.Add(m);
                dc.Configuration.ValidateOnSaveEnabled = false;
                dc.SaveChanges();

            }

            return RedirectToAction("AddPartner", "Project", e.projektID );
        }
        [NonAction]
        public List<string>  Emailliste(int projektID)
        {
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var j = dc.Memebre.Where(a => a.ProjectID == projektID).ToList();
                List<string> t = new List<string>();
                t.Add("Users added in the Project : " + dc.Projects.Where(z => z.Id == projektID).FirstOrDefault().Name + "\n --------------------------------------------------------------- \n");
                if(j.Count > 0)
                {
                    for (int i = 0; i < j.Count; i++)
                    {
                        int u = new int();
                        u = j[i].UserID;
                        var s = dc.Users.Where(w => w.UserID == u).FirstOrDefault().Email;
                        t.Add(s + "\n --------------------------------------------------------------- \n");
                    }
                }
                
                return t;
            }

        }

        [HttpGet]
        public ActionResult Testvideo()
        {
            String mail = Request.Cookies["Mycookie"].Value;
            Users u = new Users();
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == mail).FirstOrDefault();
                ViewBag.name = v.FirstName;
                return View();
            }

           
        }

    }
}