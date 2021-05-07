using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bookselling.Models
{
    public class Add_Employee
    {

    //    [Remote("IsempExists", "Employee", ErrorMessage = "Employee user name already in use")]
        [Required(ErrorMessage = "A username is required")]
        public string username { get; set; }

       
        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; }
        public string Role { get; set; }

        [Required(ErrorMessage = "A employee name is required")]
        public string Emp_Name { get; set; }
        [Required(ErrorMessage = "A phone  is required")]
      //  [MinLength(11, ErrorMessage = "Length should be greater then 10")]
        public long Phone_number { get; set; }
        [Required(ErrorMessage = "A Salary is required")]
        public int Salary { get; set; }
       
    }
}