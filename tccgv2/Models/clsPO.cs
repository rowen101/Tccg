using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tccgv2.Models
{
    public class clsPOList
    {
        public string pono { get; set; }
        public string podate { get; set; }
        public string customer { get; set; }
        public decimal? baseprice { get; set; }
        public decimal? sellingprice { get; set; }
        public string PaidStatus { get; set; }
    }

    public class clsPOSetup
    {
        [Display(Name="PO #")]
        public string ponum { get; set; }

        [Display(Name = "PO Date")]
        public string podate { get; set; }

        public string customercode { get; set; }

        public decimal? baseprice { get; set; }
        public decimal? convertedamt { get; set; }

        [Display(Name = "Customer")]
        public List<dropdown> customer { get; set; }

        public List<clsPODetails> podetail { get; set; }
    }

    public class dropdown
    {
        public string displayvalue { get; set; }
        public string valuemember { get; set; }
    }

    public class clsPODetails
    {
        public string itemcode { get; set; }
        public decimal? qty { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string vin_engine { get; set; }
        public decimal? baseprice { get; set; }
        public decimal? sellingprice { get; set; }
        public decimal? actulalamt { get; set; }
        public decimal? convertedamt { get; set; }
        public decimal? profit { get; set; }
    }
}