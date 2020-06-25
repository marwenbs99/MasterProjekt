using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using test5.Models;

namespace test5.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        
        public ActionResult Index()
        {
            String mail = Request.Cookies["Mycookie"].Value;
            var lista = new List<Projectsview>();
            
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                if (mail.Length > 0)
                {
                    var v = dc.Users.Where(a => a.Email == mail).FirstOrDefault();
                    var c = dc.Projects.Where(g => g.UserID == v.UserID).ToList();
                    ViewBag.Message = Request.Cookies["Mycookie"].Value;
                    for (int i = 0; i < c.Count; i++)
                    {
                        Projectsview pw = new Projectsview();
                        pw.Name = c[i].Name;
                        pw.Description = c[i].Description;
                        pw.ManagerName = v.FirstName + " " + v.LastName;
                        pw.IfManager = true;
                        pw.Statut = c[i].Statut;
                        pw.ID = c[i].Id;
                        lista.Add(pw);

                    }
                    var o = dc.Memebre.Where(k => k.UserID == v.UserID).ToList();
                    if (o.Count > 0)
                    {
                        for (int i = 0; i < o.Count; i++)
                        {
                            Projectsview pw = new Projectsview();
                            pw.ManagerName = ProjektManagerName(o[i].ProjectID);
                            pw.Name = Projektdetaille(o[i].ProjectID).Name;
                            pw.Description = Projektdetaille(o[i].ProjectID).Description;
                            pw.IfManager = false;
                            pw.Statut = Projektdetaille(o[i].ProjectID).Statut;
                            pw.ID = o[i].ProjectID;
                            lista.Add(pw);

                        }
                    }
                }
               
                
              
               
                return View(lista);
            }
            
        }
        [HttpGet]
        public ActionResult EditProject(int ID, String Statut)
        {
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Projects.Where(a => a.Id == ID).FirstOrDefault();
                var lista = new List<String>();
                lista.Add("To Do");
                lista.Add("Finished");
                lista.Add("	Not defined");
                ViewBag.Statut = new SelectList (lista);
                return View(v);
            }
           
        }
        [HttpPost]
        public ActionResult EditProject(Projects pj)
        {
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var projet = dc.Projects.Where(a => a.Id == pj.Id).FirstOrDefault();
                projet.Name = pj.Name;
                projet.Description = pj.Description;
                projet.Statut = pj.Statut;
                dc.Configuration.ValidateOnSaveEnabled = false;
                dc.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            
        }

        [Authorize]
        public ActionResult MyCalendar()
        {
            ViewBag.Message = "Calendar info";

            return View();
        }


        public void AddEvents(Events e)
        {
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                e.IsFullDay = false;
                e.ThemeColor = "Null";
                dc.Events.Add(e);
                dc.Configuration.ValidateOnSaveEnabled = false;
                dc.SaveChanges();
            
            }
        }

        // string connectionString = @"data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\MyDataBase.mdf;integrated security=True";
        string connectionString = @"workstation id = MSDatabase.mssql.somee.com; packet size = 4096; user id = bensalemarwen_SQLLogin_1; pwd=4zhfkgiwpq;data source = MSDatabase.mssql.somee.com; persist security info=False";
        public JsonResult GetEvents()
        {

            DataTable events = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var sqlDa = new SqlDataAdapter("SELECT * FROM Events", sqlCon);
                sqlDa.Fill(events);
                //}

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in events.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in events.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }

                    rows.Add(row);
                }

                return new JsonResult { Data = rows.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [NonAction]
        public String ProjektManagerName(int projektID)
        {
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Projects.Where(a => a.Id == projektID).FirstOrDefault();
                var u = dc.Users.Where(b => b.UserID == v.UserID).FirstOrDefault();
                return u.FirstName + " " + u.LastName;
            }
            
        }
        [NonAction]
        public Projects Projektdetaille(int projektID)
        {
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Projects.Where(a => a.Id == projektID).FirstOrDefault();
                
                return v;
            }

        }



    }
}