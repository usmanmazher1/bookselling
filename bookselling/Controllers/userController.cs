using bookselling.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace bookselling.Controllers
{
    public class userController : Controller
    {
        // GET: user
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User a)
        {
            using (var data = new BooksaleEntities2())
            {
                bool iscontain = data.Users.Any(x => x.username == a.username && x.Password == a.Password);
                if (iscontain)
                {

                    String myrole = data.Users.Where(x => x.username == a.username).Select(x => x.Role).Single();
                    if (myrole == "Admin")
                    {
                        FormsAuthentication.SetAuthCookie(a.username, false);
                        return RedirectToAction("Index", "Employee");

                    }
                    else if (myrole == "Employee")
                    {
                        FormsAuthentication.SetAuthCookie(a.username, false);
                        return RedirectToAction("Index", "Book");
                    }


                }
                else
                {
                    ModelState.AddModelError("", "invalid user or password");
                    return View();
                }

                return View();
            }
        }

        public ActionResult SIgnIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SIgnIn(User a)
        {
            using (var data = new BooksaleEntities2())
            {
                bool iscontain = data.Users.Any(x => x.username == a.username && x.Password == a.Password);
                if (iscontain)
                {
                    
                    String myrole =data.Users.Where(x=> x.username==a.username).Select(x=> x.Role).Single();
                    if (myrole=="Admin")
                    {
                        FormsAuthentication.SetAuthCookie(a.username, false);
                        return RedirectToAction("Index", "Employee");

                    }
                    else if (myrole == "Employee")
                    {
                        FormsAuthentication.SetAuthCookie(a.username, false);
                        return RedirectToAction("Index", "Book");
                    }

                    
                }
                else
                {
                    ModelState.AddModelError("", "invalid user or password");
                    return View();
                }

                return View();
            }
        }


        public ActionResult Home()
        {
            return View();
        }

        public ActionResult HomeDelivery()
        {

            return View();
        }

        public ActionResult Home1()
        {
            return View();
        }

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult forget()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AjaxMethod(string username, string password)
        {
            string a = username;
            string b= password;
            string res = "";
            using (var data = new BooksaleEntities2())
            {
               // User abc = new User();
                User user = data.Users.Where(x => x.username == a).First();
                User abc = data.Users.Find(user.user_ID);
                
                if (user == null)
                {
                    res = "Username Incorrect";
                    return Json(res);
                }
                else
                {
                    
                    abc.username = username;
                    abc.Password = password;
                    abc.Role = user.Role;
                    data.Entry(abc).State = EntityState.Modified;
                    data.SaveChanges();
                    res = "Password Changed Successfull";
                    return Json(res);
                }


            }


                
        }
    }
}