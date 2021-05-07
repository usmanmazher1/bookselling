using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bookselling.Models
{
    public class cart
    {
       
        public int Book_ID { get; set; }
        public System.DateTime Bill_Date { get; set; }
        public int Total_Price { get; set; }
        public int Quantity { get; set; }
        public virtual Book Book { get; set; }
       


    }
}