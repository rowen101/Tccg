using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tccgv2.Models
{
    public class bookinglist
    {
        public string bookingnum { get; set; }
        public string invoicenum { get; set; }
        public string bill_landing { get; set; }
        public string vessel { get; set; }
        public string port_landing { get; set; }
        public string port_descharge { get; set; }
    }


    public class bbooking_setup
    {
        [Display(Name="BOOKING #")]
        [Required(ErrorMessage="Booking # is required!")]
        public string booking_num { get; set; }

        [Display(Name = "INVOICE #")]
        [Required(ErrorMessage = "Invoice # is required!")]
        public string invoice_num { get; set; }

        [Display(Name = "BILL OF LADING NUMBER")]
        [Required(ErrorMessage = "Bill of landing # is required!")]
        public string bill_landing { get; set; }

        [Display(Name = "SHIPPER/EXPORTER")]
        [Required(ErrorMessage = "SHIPPER/EXPORTER is required!")]
        public string shipper { get; set; }

        [Display(Name = "CONSIGNEE")]
        [Required(ErrorMessage = "CONSIGNEE is required!")]
        public string consignee { get; set; }

        [Display(Name = "VESSEL / VOYAGE")]
        [Required(ErrorMessage = "VESSEL / VOYAGE is required!")]
        public string vessel { get; set; }

        [Display(Name = "PORT OF LOADING")]
        [Required(ErrorMessage = "PORT OF LOADING is required!")]
        public string port_landing { get; set; }

        [Display(Name = "PORT OF DISCHARGE")]
        [Required(ErrorMessage = "PORT OF DISCHARGE is required!")]
        public string port_descharge { get; set; }

    }
}