using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bookselling.Models;

namespace bookselling.Controllers
{
      [Authorize(Roles = "Employee")]
    public class BookController : Controller
    {
        private BooksaleEntities2 db = new BooksaleEntities2();
        public ActionResult check()
        {
           
            return View();
        }
        
        // GET: Book
        public ActionResult Index(string searchBy, string search)
        {
            var book_Details = db.Books.Include(b=> b.Employee1);
            return View(book_Details.Where(x => x.Book_Name.StartsWith(search) || search == null).ToList());
        }
        


        // GET: Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book_Details = db.Books.Find(id);
            if (book_Details == null)
            {
                return HttpNotFound();
            }
            return View(book_Details);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            ViewBag.Employee = new SelectList(db.Employees, "Emp_ID", "Emp_Name");
            ViewBag.Publisher = new SelectList(db.Sellers, "Seller_ID", "Publisher");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Book_ID,Book_Name,Cost_Price,Publisher,Sell_price,Quantity,Employee")] Book book_Details)
        {
            if (ModelState.IsValid)
            {
                book_Details.Genre = "";
                db.Books.Add(book_Details);
                db.SaveChanges();

                int lastid = db.Books.Max(item => item.Book_ID);
                Seller_Bill sb = new Seller_Bill();
                sb.Book = lastid;
                sb.Publisher = book_Details.Publisher;
                sb.Date = System.DateTime.Now;
                int amount = book_Details.Quantity * book_Details.Cost_Price;

                sb.Total_Amount = amount;

                db.Seller_Bill.Add(sb);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Employee = new SelectList(db.Employees, "Emp_ID", "Emp_Name", book_Details.Employee);
            ViewBag.Publisher = new SelectList(db.Sellers, "Seller_ID", "Publisher");
            return View(book_Details);
        }
        public JsonResult IsbookExists(string book)
        {
            BooksaleEntities2 db = new BooksaleEntities2();
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            return Json(!db.Books.Any(x => x.Book_Name == book), JsonRequestBehavior.AllowGet);
        }
        // GET: Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book_Details = db.Books.Find(id);
            if (book_Details == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee = new SelectList(db.Employees, "Emp_ID", "Emp_Name", book_Details.Employee);
            return View(book_Details);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_ID,General,Cost_Price,Publisher,Sell_price,Quantity,Employee")] Book book_Details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book_Details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee = new SelectList(db.Employees, "Emp_ID", "Emp_Name", book_Details.Employee);
            return View(book_Details);
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book_Details = db.Books.Find(id);
            db.Books.Remove(book_Details);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
