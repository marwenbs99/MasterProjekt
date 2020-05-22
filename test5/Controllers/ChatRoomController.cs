using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test5.Models;
using System.Data.Entity;
using System.Diagnostics;

namespace test5.Controllers
{
    public class ChatRoomController : Controller
    {
        [Authorize]
        // GET: ChatRoom
        public ActionResult UserComment()
        {
            Users us = new Users();
             var lista = new List<Comments>();
            using(MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                  lista = dc.Comments.Include(x => x.Replies).OrderByDescending(x => x.CreatedOn).ToList();

                
                
                foreach(var list in lista)
                {
                    list.Users = dc.Users.Where(a => a.UserID == list.UserID).FirstOrDefault();   

                }
               
            }
            return View(lista);

        }

        [HttpPost]
        public ActionResult PostReply(ReplyVM rp)
        {
            Replies rep = new Replies();
            rep.Text = rp.RelyText;
            rep.CommentID = rp.CID;
            string email = Request.Cookies["Mycookie"].Value;
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                rep.UserID = v.UserID;
                rep.CreatedOn = DateTime.Now;
                dc.Replies.Add(rep);
                dc.SaveChanges();
                
            }
            return RedirectToAction("UserComment");
        }



        public ActionResult PostComment(String commentText)
        {
            Comments cm = new Comments();
            cm.Text = commentText;
            
            string email = Request.Cookies["Mycookie"].Value;
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                cm.UserID = v.UserID;
                cm.CreatedOn = DateTime.Now;
                dc.Comments.Add(cm);
                dc.SaveChanges();

            }
            return RedirectToAction("UserComment");
        }


    }
}