using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tccgv2.Models
{
    public class PaymentList
    {
        public string _PAY_NUM { get; set; }
        public string _PAY_DTE { get; set; }
        public string _INVOICE_NUM { get; set; }
        public string _CUSTOMER_NAME { get; set; }
        public decimal? _AMT_PAID { get; set; }
        public decimal? _PAY_BALANCE { get; set; }
    }

    public class InvoiceInfo
    {
        public string invoicenum { get; set; }
        public decimal? totalpayable { get; set; }
    }

 

}