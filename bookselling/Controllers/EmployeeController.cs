using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using bookselling.Models;


namespace bookselling.Controllers
{
    [Authorize(Roles="Admin")]
    public class EmployeeController : Controller
    {
        // GET: Employee
        private BooksaleEntities2 db = new BooksaleEntities2();
        
        public ActionResult Index(String search)
        {
            var emp_Details = db.Employees.Include(a=> a.User);
            return View(emp_Details.Where(x => x.Emp_Name.StartsWith(search) || search == null).ToList());
        }

        public ActionResult Purchase_Bill()
        {
            var Seller_bill = db.Seller_Bill.Include(a => a.Book1).Include(a=> a.Seller);
            return View(Seller_bill.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Add_Employee add_emp)
        {
            if (ModelState.IsValid)
            {
                User user = new User();

                user.username = add_emp.username;
                user.Password = add_emp.Password;
                user.Role = "Employee";
                db.Users.Add(user);
                db.SaveChanges();
                int user_ID = db.Users.Max(item => item.user_ID);

                Employee emp = new Employee();
                emp.Emp_Name = add_emp.Emp_Name;
                emp.Phone_Number = add_emp.Phone_number;
                emp.Salary = add_emp.Salary;
                emp.users_ID = user_ID;
                db.Employees.Add(emp);
                db.SaveChanges();




                return RedirectToAction("Index");
            }


            return View(add_emp);

        }
        public JsonResult IsempExists(string emp)
        {
            BooksaleEntities2 db = new BooksaleEntities2();
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            return Json(!db.Employees.Any(x => x.User.username == emp), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee emp_Details = db.Employees.Find(id);
            if (emp_Details == null)
            {
                return HttpNotFound();
            }

            
            return View(emp_Details);
        }


        [HttpPost]
        public ActionResult Edit(Employee emp_Details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emp_Details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(emp_Details);
        }
    }
}