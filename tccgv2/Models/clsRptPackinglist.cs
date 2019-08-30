using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tccgv2.Models
{
    public class clsRptPackinglist
    {
        public string plnum { get; set; }
        public string pldte { get; set; }
        public string customer { get; set; }
        public string via { get; set; }
        public string ship { get; set; }
        public string customername { get; set; }
        public decimal? gross_weight { get; set; }
        public List<rpt_packinglist_dtl> pklist { get; set; }
    }

    public class rpt_packinglist_dtl
    {
        public decimal? qty { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string vin { get; set; }
        public decimal? net_weight { get; set; }
    }
}