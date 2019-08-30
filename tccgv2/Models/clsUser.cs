using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace tccgv2.Models
{
    public class TccgLogin
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        public string uname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string upass { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class Userlist
    {
        public string username { get; set; }
        public string fullname { get; set; }
        public string designation { get; set; }
        public string email { get; set; }
        public string Group { get; set; }
        public string isactive { get; set; }
    }

    public class UserSetup
    {
        [Display(Name="Full Name")]
        public string fullname { get; set; }

        [Display(Name = "Username")]
        public string uname { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password")]
        [Display(Name="Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        public List<AccessList> accesslst { get; set; }

    }

    public class AccessList
    {
        public string menuid { get; set; }
        public string menuname { get; set; }
        public bool ischeck { get; set; }
    }
}