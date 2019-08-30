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
   

    public class clsClientList
    {
        public string client_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string inquirydate { get; set; }
        public string iscustomer { get; set; }
    }

    public class clsClientSetup
    {
        [Display(Name = "Client Name")]
        [Required(ErrorMessage = "Client name is required!")]  
        public string CLIENT_NAME { get; set; }

        [Display(Name = "Address")]
        public string CLIENT_ADDRESS { get; set; }

        [Display(Name = "Tel")]
        [DataType(DataType.PhoneNumber)]
        public string CLIENT_TEL { get; set; }

        [Display(Name = "Fax")]
        [DataType(DataType.PhoneNumber)]
        public string CLIENT_FAX { get; set; }

        [Display(Name = "Mobile")]
        public string CLIENT_MOBILE { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string CLIENT_EMAIL { get; set; }

        [Display(Name = "Inquiry Details")]
        [Required(ErrorMessage="Inquiry Details is required!")]    
        public string INQUIRY_DETAILS { get; set; }

        [Display(Name = "Inquiry Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage="inquiry date is required!")]
        public DateTime? INQUIRY_DATE { get; set; }

        [Display(Name = "Desposition")]
        public string DISPOSITION { get; set; }

        [Display(Name = "Remarks")]
        public string REMARKS { get; set; }

        [Display(Name = "Is Customer")]
        public bool? ISCUSTOMER { get; set; }
    }


}