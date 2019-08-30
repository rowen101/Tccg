using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tccgv2.Models
{
    public class clsExpense
    {
        public string invoicenum { get; set; }
        public decimal? expenseamt { get; set; }
    }

    public class clsExpenseDtl
    {
        public string invoicenum { get; set; }
        public string trans_date { get; set; }
        public string trans_des { get; set; }
        public decimal? trans_amt { get; set; }
    }
   
    public class expense_setup
    {
        public string customercode { get; set; }
        public string invoice_num { get; set; }
        public decimal? total_amt { get; set; }
        public List<expense_setup_dtl> expensedtl { get; set; }

    }

    public class expense_setup_dtl
    {
        public string trans_des { get; set; }
        public string trans_dte { get; set; }
        public decimal? trans_amt { get; set; }
    }
}