using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tccgv2.Models
{
    public class preview_invoice
    {
        public string invoice_num { get; set; }
        public string invoice_dte { get; set; }
        public decimal? total_amt { get; set; }
        public List<preview_invoice_dtl> invoice_dtl { get; set; }
    }
    
    public class preview_invoice_dtl
    {
        public decimal? qty { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string vin { get; set; }
        public decimal? price { get; set; }
    }
}