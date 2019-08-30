using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace tccgv2.Models
{
    public class clsInvoiceList
    {
        public string Invoice_num { get; set; }
        public string Invoice_date { get; set; }
        public string Customer_name { get; set; }
        public string Total_amt { get; set; }
        public string status { get; set; }
        public string PaidStatus { get; set; }
    }

    public class clsInvoiceSetup
    {
        [Display(Name="Invoice #:")]
        public string Invoice_num { get; set; }

        [Display(Name = "Create Date:")]
        public string InvoiceDte { get; set; }

        [Display(Name = "Customer:")]
        public List<dropdown> customerlist { get; set; }

        [Display(Name = "Ship:")]
        public string Ship { get; set; }

        [Display(Name = "Via:")]
        public string Via { get; set; }
    }

    public class Invoicesave
    {
        public string Invoice_num { get; set; }
        public DateTime? Invoice_date { get; set; }
        public string cust_code { get; set; }
        public decimal? Total_amt { get; set; }
        public string status { get; set; }
        public string ship { get; set; }
        public string via { get; set; }

        public List<Invoice_details_save> itemdetails { get; set; }
    }

    public class Invoice_details_save
    {
        public string ponum { get; set; }
        public decimal? qty { get; set; }
        public string itmcode { get; set; }
        public decimal? price { get; set; }
        public decimal? amount { get; set; }

    }

    public class Invoice_Details
    {
        public string refpo { get; set; }
        public decimal? qty { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string vin { get; set; }
        public decimal? price { get; set; }
        public decimal? runingtot { get; set; }
        public string itmcode { get; set; }
    }
}