using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using test5.Models;
using System.Data.SqlClient;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using System.IO;
using WebGrease.Css.Ast.Selectors;
using System.Web.UI.WebControls;
using Microsoft.Extensions.Caching.Memory;
using System.Web.Routing;

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
                    ModelState.AddModelError("Email Exist", "Email is already Exist");
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
                    SendVerificationMail(user.Email, user.ActiovationCode.ToString(), "VerifyAccount");
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
        public ActionResult VerifyAccount(String ID)
        {
            bool Status = false;
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {

                dc.Configuration.ValidateOnSaveEnabled = false; // This line i have added here to avoid confirm password does not match issue on save changes
                var v = dc.Users.Where(a => a.ActiovationCode == new Guid(ID)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;

                }
                else
                {
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
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            bool Status = false;
            string message = "";
            ViewBag.Message = message;
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == login.Email).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(Crypto.Hash(login.password), v.Password) == 0)
                    {
                        if (v.IsEmailVerified == true)
                        {
                            int timeout = login.rememberMe ? 525600 : 1; // 525600 min = 1 Year
                            var ticket = new FormsAuthenticationTicket(v.FirstName, login.rememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);

                            HttpCookie cookie1 = new HttpCookie("Mycookie");
                            cookie1.Value = v.Email;
                            cookie1.Expires = DateTime.Now.AddDays(30);
                            cookie1.HttpOnly = true;
                            Response.Cookies.Add(cookie1);




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


        }

        [NonAction]
        public bool IsEmailExist(String Email)
        {
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == Email).FirstOrDefault();
                return v != null;
            }
        }

        //SendVerification Email procedure
        [NonAction]
        public void SendVerificationMail(String mail, string ActivationCode, string emailfor)
        {
            var VerifyUrl = "/User/" + emailfor + "/" + ActivationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, VerifyUrl);
            var fromEmail = new MailAddress("coboye1992@hotmail.fr", "Master Projekt");
            var toEmail = new MailAddress(mail);
            var fromEmailPassword = "97815709";
            String subject = "";
            String body = "";

            if (emailfor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/> We are excited to tell you that your account is" + "successfully created. Please click on the below link to verify your account" + "<br/><br/> <a href='" + link + "'>" + link + "</a>";
            }
            else if (emailfor == "ResetPassword")
            {
                subject = "Password Reset";
                body = "Hi,<br/><br/>We got request for reset your account-password. Please click on the below link to reset your password " + "<br/><br/><a href=" + link + "> Reset Password link </a>";
            }

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

        //Password forget--------------------------------------------------------------------------------
        public ActionResult Forgotpassword()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Forgotpassword(string email)
        {
            //verify email exist
            //generate reset password link
            //send Email
            string message = "";

            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var account = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                if (account != null)
                {
                    //send Email for reset password
                    string ResetCode = Guid.NewGuid().ToString();
                    SendVerificationMail(account.Email, ResetCode, "ResetPassword");
                    account.ResetPasswordCode = ResetCode;
                    //This line i have added here to avoid confirm password not mutch issue, as we have added a confirm password proprerty
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset Password link has been sent to your Email";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;

            return View();
        }



        public ActionResult ResetPassword(string ID)
        {

            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.ResetPasswordCode == ID).FirstOrDefault();
                if (v != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = ID;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }

            }


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (model.NewPassword.Length >= 6 && model.NewPassword == model.ConfirmPassword)
            {


                if (ModelState.IsValid)
                {
                    using (MyDataBaseEntities dc = new MyDataBaseEntities())
                    {
                        var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                        if (user != null)
                        {
                            user.Password = Crypto.Hash(model.NewPassword);
                            user.ResetPasswordCode = "";
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                            message = "New Password update succesfully";
                            return RedirectToAction("Login", "User");
                        }

                    }

                }
                else
                {
                    message = "Something invalide ";
                }


            }



            ViewBag.Message = message;

            return View(model);
        }
        [Authorize]
        [HttpGet]
        public ActionResult EditProfile()
        {




            UserProfile up = new UserProfile();
            string mail = "";
            mail = Request.Cookies["Mycookie"].Value;
            using (MyDataBaseEntities dc = new MyDataBaseEntities())
            {
                var v = dc.Users.Where(a => a.Email == mail).FirstOrDefault();
                ViewBag.that = v.FirstName + " " + v.LastName;

                if (v.ImageUrl != "")
                {

                    up.ImageUrl = v.ImageUrl;
                    return View(up);
                }

            }


            return View();
        }
        [HttpPost]
        public ActionResult EditProfile(UserProfile up, string submit)
        {

            if (submit == "Update Password")
            {

                string email = Request.Cookies["Mycookie"].Value;
                using (MyDataBaseEntities dc = new MyDataBaseEntities())
                {
                    var v = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                    if (ModelState.IsValid)
                    {




                        String oldpassword = Crypto.Hash(up.OldPassword);
                        if (oldpassword == v.Password)
                        {
                            v.Password = Crypto.Hash(up.password);
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                            ViewBag.Message = "New Password update succesfully";
                            up.ImageUrl = v.ImageUrl;
                            return View(up);

                        }
                        else
                        {
                            ViewBag.Message = "Old password is wrong!";
                            up.ImageUrl = v.ImageUrl;
                            return View(up);
                        }





                    }
                    else
                    {
                        ViewBag.Message = "Something invalide";
                        up.ImageUrl = v.ImageUrl;
                        return View(up);

                    }
                }


            }
            else if (submit == "Update Picture")
            {


                string message = "";
                string email = Request.Cookies["Mycookie"].Value;

                using (MyDataBaseEntities dc = new MyDataBaseEntities())
                {
                    var v = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                    ViewBag.that = v.FirstName + " " + v.LastName;


                    //---------------------------------------------
                    var file = up.Picture;
                    var extention = Path.GetExtension(file.FileName);
                    var extentionplusid = v.UserID + extention;
                    String ImgUrl = "~/Profile Image/" + extentionplusid;
                    v.ImageUrl = ImgUrl;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();

                    //---------------------------------------------



                    var path = Server.MapPath("~/Profile Image/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (System.IO.File.Exists(path + extentionplusid))
                    {
                        System.IO.File.Delete(path + extentionplusid);
                    }
                    file.SaveAs(path + extentionplusid);
                    ViewBag.Message = "Your Picture is update succesfully";


                }


                
                return RedirectToAction("EditProfile", "User");

            }

            return View(up);
            //return RedirectToAction("EditProfile", "User");


        }








    }
}