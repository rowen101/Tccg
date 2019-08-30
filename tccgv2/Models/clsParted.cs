using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tccgv2.Models
{

    public class partedlist
    {
        public string ponum { get; set; }
        public decimal? total_profit { get; set; }
    }

    public class perted_personlist
    {
        public string person { get; set; }
        public decimal? total_rate { get; set; }
    }

    public class partedsetup
    {
        public string ponum { get; set; }
        public string parted_person { get; set; }

        public List<perted_list> prted_list { get; set; }
    }

    public class perted_list
    {
        public decimal? qty { get; set; }
        public string itemdesc { get; set; }
        public decimal? profit { get; set; }
        public decimal? rate_percent { get; set; }
    }
}