using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.Security;
using test5.Models;
using System.Data.SqlClient;
using System.Security.Principal;

namespace test5.Controllers
{
    public class UserController : Controller
    {



        //Registratiom Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration Post Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActiovationCode")]Users user)
        {
            
            bool Status = false;
            String message = "";
            if (ModelState.IsValid)
            {
                #region //Email is already Exist
                var IsExist = IsEmailExist(user.Email);
                if (IsExist)
                {
                    ModelState.AddModelError("Email Exist","Email is already Exist");
                    return View(user);
                }
                #endregion
                #region //generate Actiovation Code
                user.ActiovationCode = Guid.NewGuid();
                #endregion
                #region //Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion
                user.IsEmailVerified = false;
                #region //Save to DataBase
                
                using (MyDataBaseEntities dc = new MyDataBaseEntities())
                {

                    dc.Users.Add(user);
                   
                    
                    dc.SaveChanges();
                    //send Email to user
                    SendVerificationMail(user.Email, user.ActiovationCode.ToString());
                    message = "Registration successfully done, Account activation link" + "Has been sent to your email adresse :" + user.Email;
                    Status = true;


                }

                #endregion
            }
            else
            {
                message = "Invalid request";
            }




            ViewBag.Message = message;
            ViewBag.Status = Status;
        
            return View(user);
        }
        // Verify account
        [HttpGet]
        public ActionResult VerifyAccount(String ID) {
            bool Status = false;
            using (MyDataBaseEntities dc = new MyDataBaseEntities()) {

                dc.Configuration.ValidateOnSaveEnabled = false; // This line i have added here to avoid confirm password does not match issue on save changes
                var v = dc.Users.Where(a => a.ActiovationCode == new Guid(ID)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;

                }
                else {
                    ViewBag.Message = "Invalide request";
                }
            }

            ViewBag.Status = Status;
            return View();
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl="")
        {
            bool Status = false;
            string message = "";
            ViewBag.Message = message;
            using (MyDataBaseEntities dc = new MyDataBaseEntities()) {
                var v = dc.Users.Where(a => a.Email == login.Email).FirstOrDefault();
                if (v !=null)
                {
                    if (string.Compare(Crypto.Hash(login.password),v.Password) == 0 )
                    {
                        if (v.IsEmailVerified == true)
                        {
                            int timeout = login.rememberMe ? 525600 : 1; // 525600 min = 1 Year
                            var ticket = new FormsAuthenticationTicket(login.Email, login.rememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                Status = true;
                                return Redirect(ReturnUrl);

                            }
                            else
                            {
                                Status = true;
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            message = "Email most be verified firsrt";
                        }
                       
                    }
                    else
                    {
                        message = " invalid credential provided";
                    }

                }
                else
                {
                    message = " invalid credential provided";
                }
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }
        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");

          return View();
        }
        [NonAction]
        public bool IsEmailExist(String Email)
        { 
            using (MyDataBaseEntities dc = new MyDataBaseEntities() )
            {
                var v = dc.Users.Where(a => a.Email == Email).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationMail(String mail, string ActivationCode)
        {
            var VerifyUrl = "/User/VerifyAccount/" + ActivationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, VerifyUrl);
            var fromEmail = new MailAddress("coboye1992@hotmail.fr", "Master Projekt");
            var toEmail = new MailAddress(mail);
            var fromEmailPassword = "97815709";
            String subject = "Your account is successfully created!";
            String body = "<br/><br/> We are excited to tell you that your account is"+ "successfully created. Please click on the below link to verify your account"+ "<br/><br/> <a href='" + link+"'>"+link+"</a>";
            var smtp = new SmtpClient
            {
                Host = "smtp.live.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)


            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
                smtp.Send(message);

        }
    }
    
}