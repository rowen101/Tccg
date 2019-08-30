using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace tccgv2.Models
{
    public class packinglist
    {
        [Display(Name="Packing List #:")]
        public string pl_num { get; set; }

        [Display(Name = "PL Date:")]
        public string pl_date { get; set; }
        public decimal? gross { get; set; }
        public string invoice_num { get; set; }
    }

    public class packinglist_dtl
    {
        public decimal? qty { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string vin { get; set; }
        public string itmcode { get; set; }
        public decimal? weigth { get; set; }
    }


    public class plsv_mstr
    {
        public string pl_num { get; set; }
        public string pl_date { get; set; }
        public string invoice_num { get; set; }
        public decimal? gross { get; set; }

        public List<plsv_dtl> dtl { get; set; }
    }
     
    public class plsv_dtl
    {
        public string itmcde { get; set; }
        public decimal? kg { get; set; }
    }
}