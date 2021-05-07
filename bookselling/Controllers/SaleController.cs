using bookselling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;

namespace bookselling.Controllers
{
    
    public class SaleController : Controller
    {
        private BooksaleEntities2 db = new BooksaleEntities2();
        // GET: Sale
        public ActionResult Index(int? cid)
        {

            
            return View(db.Bills.ToList());
        }

        public ActionResult Invoice()
        {
            
            return View(db.Invoices.ToList());
        }

        public ActionResult order()
        {

            if ((List<cart>)Session["cart"] == null)
            {

                return RedirectToAction("Create", "Sale");
            }


            return View((List<cart>)Session["cart"]);
        }

        public ActionResult Remove(int? id)
        {
            List<cart> li = (List<cart>)Session["cart"];
            li.RemoveAll(x => x.Book_ID == id);
            Session["cart"] = li;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return RedirectToAction("order", "Sale");

        }

        public ActionResult Create()
        {

            ViewBag.Book_ID = new SelectList(db.Books, "Book_ID", "Book_Name");
            return View();
        }

        [HttpPost]
        public JsonResult AjaxMethod(string value)
        {
            int val = int.Parse(value);
            string price = db.Books.Where(x => x.Book_ID == val).Select(x => x.Sell_price).SingleOrDefault().ToString();

            
            return Json(price);
        }
        [HttpPost]
        public ActionResult Create(cart c)
        {
            int QTY = c.Quantity;
            int bk_ID = c.Book_ID;
            string actualQTY = db.Books.Where(a => a.Book_ID == bk_ID).Select(a => a.Quantity).SingleOrDefault().ToString();

            if (int.Parse(actualQTY) < QTY)
            {
                Session["QTY"] = "Book Not Available in Stock! Available Book is :" + actualQTY;

                ViewBag.Book_ID = new SelectList(db.Books, "Book_ID", "Book_Name");
                return RedirectToAction("Create", "Sale");
            }
            else
            {
                Session.Remove("QTY");

            }


            if (Session["cart"] == null)
            {
               
               
                    List<cart> li = new List<cart>();

                    li.Add(c);
                    Session["cart"] = li;
                    ViewBag.cart = li.Count();


                    Session["count"] = 1;

                

                


            }
            else
            {
                
                List<cart> li = (List<cart>)Session["cart"];
                li.Add(c);
                Session["cart"] = li;
                ViewBag.cart = li.Count();
                Session["count"] = Convert.ToInt32(Session["count"]) + 1;
               
            }
            ViewBag.Book_ID = new SelectList(db.Books, "Book_ID", "Book_Name");
            return RedirectToAction("Create", "Sale");



            /*if (ModelState.IsValid)
            {
                string b = Bill_Det.Book_ID.ToString();
               // string a = Bill_Det.Employee_ID.ToString();
                db.Bills.Add(Bill_Det);
                db.SaveChanges();
                GenerateInvoicePDF(Bill_Det.Bill_ID);

                int QTY = Bill_Det.Quantity;
                int bk_ID = Bill_Det.Book_ID;
                string actualQTY = db.Book_Details.Where(x => x.Book_ID == bk_ID).Select(x=> x.Quantity).SingleOrDefault().ToString();

              

                int remaning = int.Parse(actualQTY) - QTY;

                var  BD = new Book_Details() {Book_ID = bk_ID, Quantity = remaning.ToString() };
                db.Book_Details.Attach(BD);

                db.Entry(BD).Property(x=> x.Quantity).IsModified =true;
                
                db.SaveChanges();


                return RedirectToAction("Index");
            }*/

            
          
        }


        public ActionResult confirmorder()
        {
            if (Session["cart"] == null)
            {


            }
            else
            {
                int sum = 0;

                List<cart> li = (List<cart>)Session["cart"];
                foreach(var x in li)
                {
                    sum = Convert.ToInt32(x.Total_Price) + sum;

                }
                Invoice inc = new Invoice();
                inc.Total_Amount = sum;
                inc.Bill_Date = System.DateTime.Now;
                inc.Employee = 2;

                db.Invoices.Add(inc);
                db.SaveChanges();

                int invoice = db.Invoices.Max(item => item.Invoice1);

                foreach (var x in li)
                {


                    Bill bill = new Bill();
                    bill.Book_ID = x.Book_ID;
                    bill.Bill_Date = x.Bill_Date;
                    bill.Total_Price = x.Total_Price;
                    bill.Quantity = x.Quantity;
                    bill.Invoice = invoice;

                    db.Bills.Add(bill);
                    db.SaveChanges();

                    int QTY = x.Quantity;
                    int bk_ID = x.Book_ID;
                    string actualQTY = db.Books.Where(a => a.Book_ID == bk_ID).Select(a => a.Quantity).SingleOrDefault().ToString();



                    int remaning = int.Parse(actualQTY) - QTY;

                  var query = (from q in db.Books
							where q.Book_ID == bk_ID
							select q).First();
				query.Quantity = remaning;
				int result = db.SaveChanges();

                    
                    GenerateInvoicePDF();

                }



            }


            return RedirectToAction("Index");
        }

        protected void GenerateInvoicePDF()
        {
            //Dummy data for Invoice (Bill).


            int last = db.Bills.Max(item => item.Bill_ID);
            int invoice = db.Bills.Where(x => x.Bill_ID == last).Select(x => x.Invoice).SingleOrDefault();
            string companyName = "ABC Company";
            int orderNo = invoice;
         


            DataTable dt = new DataTable();
            

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Order Sheet</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Order No: </b>");
                    sb.Append(25);
                    sb.Append("</td><td align = 'right'><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Company Name: </b>");
                    sb.Append(companyName);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    List<cart> li = (List<cart>)Session["cart"];
                   
                    sb.Append("<table class='table'>");
                    sb.Append("<tr>");
                    sb.Append("<th>");
                    sb.Append("<b>Book</b>");
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("<b>Quantity</b>");
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("<b>Amount</b>");
                    sb.Append("</th>");
                    sb.Append("<th> </th>");
                    sb.Append("</tr>");
                    int sum = 0;
                    foreach (var x in li)
                    {
                        sum = Convert.ToInt32(x.Total_Price) + sum;

                    }
                    foreach (var x in li)
                    {
                        string books = db.Books.Where(a => a.Book_ID == x.Book_ID).Select(a => a.Book_Name).SingleOrDefault().ToString();


                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append(books);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(x.Quantity);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(x.Total_Price);
                        sb.Append("</td>");
                        sb.Append("<td> </td>");
                        sb.Append("</tr>");
                    }

                    sb.Append("<tr> <td> </td> <td> </td> <td> <b> Total Amount </b></td> ");
                    sb.Append("<td>");
                    sb.Append(sum);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");

                  









                                  //Export HTML String as PDF.
                                  StringReader sr = new StringReader(sb.ToString());
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentType = "application/pdf";
                    //Response.AddHeader("content-disposition", "attachment;filename=Invoice_" + 25 + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
    }
}