using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tccgv2.Models;
using PagedList;

namespace tccgv2.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/

        clsprocedure procedure = new clsprocedure();

        public ActionResult Index()
        {
            return View();
        }

        #region "Purchase Order"
        [ActionName("purchase-order-list")]
        public ActionResult PurchaseOrderList(int? p)
        {
            List<clsPOList> polist = new List<clsPOList>();
            TCCGDataContext dbcontext = new TCCGDataContext();

            var q_polist = from aa in dbcontext.V_PO_LISTs
                           orderby aa.PODATE descending
                           select aa;

            foreach (var row in q_polist)
            {
                polist.Add(new clsPOList { pono=row.PO_NO,podate=DateTime.Parse(row.PODATE.ToString()).ToString("MM/dd/yyyy"), customer=row.CUSTOMER_NAME,baseprice=row.BASE_PRICE,sellingprice=row.SELLING_PRICE,PaidStatus=row.Description});
            }

            int pageSize = 10;
            int pageNumber = (p ?? 1);

            return View(polist.ToPagedList(pageNumber, pageSize));
            
        }


        [ActionName("create-po")]
        public ActionResult CreatePO()
        {
            clsPOSetup posetup = new clsPOSetup();
            List<dropdown> cust = new List<dropdown>();
            TCCGDataContext dbcontext=new TCCGDataContext();
          

            //posetup.ponum = clsfunc.CodeGenerator(procedure.constring(), "po_num", "tbl_100_PO", "PO-" + DateTime.Now.ToString("MMddyy"), 10, 3);
            posetup.ponum = "[SYSTEM GENERATED]";
            posetup.podate = DateTime.Now.Date.ToShortDateString();


            var q_tccg_customer = from aa in dbcontext.TCCG_CUSTOMERs
                                  where aa.cust_status == "SID005"
                                  select new { aa.CUSTOMER_CODE, aa.CUSTOMER_NAME };



            cust.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
            foreach (var row in q_tccg_customer)
            {
                cust.Add(new dropdown { displayvalue=row.CUSTOMER_NAME,valuemember=row.CUSTOMER_CODE});
            }

            posetup.customer = cust;

            return View(posetup);
        }

        [ActionName("modify-po")]
        public ActionResult ModifyPO(string id)
        {
            clsPOSetup posetup = new clsPOSetup();
            List<dropdown> cust = new List<dropdown>();
            TCCGDataContext dbcontext = new TCCGDataContext();


            var q_po_profile = from aa in dbcontext.TCCG_PO_MSTRs
                               where aa.PO_NUM == id
                               select aa;

            posetup.ponum = id;
            posetup.podate =DateTime.Parse(q_po_profile.First().PO_DATE.ToString()).ToShortDateString();


            var q_tccg_customer = from aa in dbcontext.TCCG_CUSTOMERs
                                  where aa.cust_status == "SID005"
                                  select new { aa.CUSTOMER_CODE, aa.CUSTOMER_NAME };



            cust.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
            foreach (var row in q_tccg_customer)
            {
                cust.Add(new dropdown { displayvalue = row.CUSTOMER_NAME, valuemember = row.CUSTOMER_CODE });
            }

            posetup.customer = cust;
            ViewBag.cusid = q_po_profile.First().PO_CUSTOMER;
            return View(posetup);

        }


        #endregion


        #region "Invoice Transaction"

        [ActionName("invoice-transaction")]
        public ActionResult InvoiceTransaction(int? p)
        {
            List<clsInvoiceList> InvoiceList = new List<clsInvoiceList>();
            TCCGDataContext dbcontext = new TCCGDataContext();

            var q_invoicelist = from aa in dbcontext.V_INVOICEs
                                orderby aa.INVOICE_DTE descending
                                select aa;


            foreach (var row in q_invoicelist)
            {
                InvoiceList.Add(new clsInvoiceList { Invoice_num=row.INVOICE_NUM,
                Invoice_date=DateTime.Parse(row.INVOICE_DTE.ToString()).ToShortDateString(),
                Customer_name=row.CUSTOMER_NAME,
                Total_amt = string.Format(string.Format("{0:#,#.00}", row.DR_Total)),
                status=row.STATUS,
                PaidStatus=row.PAID_STATUS
                });
            }

            int pageSize = 10;
            int pageNumber = (p ?? 1);

            return View(InvoiceList.ToPagedList(pageNumber, pageSize));
        }

        [ActionName("create-invoice")]
        public ActionResult CreateInvoice()
        {
            TCCGDataContext dbcontext = new TCCGDataContext();
            clsInvoiceSetup invoice_setup = new clsInvoiceSetup();
            List<dropdown> cust = new List<dropdown>();

            invoice_setup.Invoice_num = "[SYSTEM GENERATED]";
            invoice_setup.InvoiceDte = DateTime.Now.Date.ToShortDateString();

            var q_tccg_customer = from aa in dbcontext.TCCG_CUSTOMERs
                                  where aa.cust_status == "SID005"
                                  select new { aa.CUSTOMER_CODE, aa.CUSTOMER_NAME };



            cust.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
            foreach (var row in q_tccg_customer)
            {
                cust.Add(new dropdown { displayvalue = row.CUSTOMER_NAME, valuemember = row.CUSTOMER_CODE });
            }

            invoice_setup.customerlist = cust;

            return View(invoice_setup);
        }

        [ActionName("modify-invoice")]
        public ActionResult ModifyInvoice(string id)
        {
            TCCGDataContext dbcontext = new TCCGDataContext();
            clsInvoiceSetup invoice_setup = new clsInvoiceSetup();
            List<dropdown> cust = new List<dropdown>();

            var q_invoice_dtl = (from aa in dbcontext.TCCG_INVOICE_MSTRs
                                where aa.DR_num == id
                                select aa).First();

            invoice_setup.Invoice_num = q_invoice_dtl.DR_num;
            invoice_setup.InvoiceDte =DateTime.Parse(q_invoice_dtl.DR_date.ToString()).ToShortDateString();
            invoice_setup.Ship = q_invoice_dtl.DR_Ship;
            invoice_setup.Via = q_invoice_dtl.DR_via;
            ViewBag.customercode = q_invoice_dtl.DR_customer;

            var q_tccg_customer = from aa in dbcontext.TCCG_CUSTOMERs
                                  where aa.cust_status == "SID005"
                                  select new { aa.CUSTOMER_CODE, aa.CUSTOMER_NAME };



            cust.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
            foreach (var row in q_tccg_customer)
            {
                cust.Add(new dropdown { displayvalue = row.CUSTOMER_NAME, valuemember = row.CUSTOMER_CODE });
            }

            invoice_setup.customerlist = cust;

            return View(invoice_setup);
        }

        #endregion


        #region "Payment Transaction"

        [ActionName("payment-transaction")]
        public ActionResult PaymentTransaction(int? p)
        {
            TCCGDataContext dbcon=new TCCGDataContext();
            List<PaymentList> pylst = new List<PaymentList>();

            var q_pay_list = from aa in dbcon.V_PAYMENT_LISTs
                             orderby aa.PAY_DTE descending
                             select aa;

            foreach (var row in q_pay_list)
            {
                pylst.Add(new PaymentList { 
                _PAY_NUM=row.PAY_NUM,
                _PAY_DTE =DateTime.Parse(row.PAY_DTE.ToString()).ToShortDateString(),
                _INVOICE_NUM=row.INVOICE_NUM,
                _CUSTOMER_NAME=row.CUSTOMER_NAME,
                _AMT_PAID=row.AMT_PAID,
                _PAY_BALANCE=row.PAY_BALANCE
                });
            }

            int pageSize = 10;
            int pageNumber = (p ?? 1);
            return View(pylst.ToPagedList(pageNumber, pageSize));
        }

        #endregion

        #region "Expense Management"
        [ActionName("expense-management")]
        public ActionResult ExpenseManagement(int? p)
        {
            TCCGDataContext dbcon = new TCCGDataContext();
            List<clsExpense> expense = new List<clsExpense>();

            var q_expense = from aa in dbcon.V_EXPENSEs
                            select aa;

            foreach (var row in q_expense)
            {
                expense.Add(new clsExpense { invoicenum=row.INVOICE_NUM,expenseamt=row.EXPENSE_AMT });
            }


            int pageSize = 10;
            int pageNumber = (p ?? 1);
            return View(expense.ToPagedList(pageNumber, pageSize));
        }

        [ActionName("create-expense")]
        public ActionResult CreateExpense()
        {
            return View();
        }

        [ActionName("modify-expense")]
        public ActionResult ModifyExpense(string invoice)
        {
            ViewBag.invoice = invoice;
            return View();
        }

#endregion

        #region "Packing list"

        [ActionName("packing-list")]
        public ActionResult PackingList(int? p)
        {
             TCCGDataContext dbcon = new TCCGDataContext();
            List<packinglist> pl_lst = new List<packinglist>();

            var q_pclist = from aa in dbcon.TCCG_PACKING_LISTs
                           select aa;

            foreach (var row in q_pclist)
            {
                pl_lst.Add(new packinglist { pl_num=row.PL_NUM,pl_date=DateTime.Parse(row.PL_DTE.ToString()).ToShortDateString(),gross=row.GROSS_WEIGHT ,invoice_num=row.INVOICE_NUM});
            }

            int pageSize = 10;
            int pageNumber = (p ?? 1);
            return View(pl_lst.ToPagedList(pageNumber, pageSize));
        }

        [ActionName("cr-pl")]
        public ActionResult CreatePackingList()
        {
            packinglist pl = new packinglist();
            pl.pl_num = "[SYSTEM GENERATED]";
            pl.pl_date = DateTime.Now.ToShortDateString();
            return View(pl);
        }

        [ActionName("md-pl")]
        public ActionResult ModifyPakingList(string id)
        {
            TCCGDataContext dbcon = new TCCGDataContext();

            var q_packinglist = (from aa in dbcon.V_PACKING_LISTs
                                where aa.PL_NUM == id
                                select aa).First();

            packinglist pl = new packinglist();
            pl.pl_num = id;
            pl.pl_date =DateTime.Parse(q_packinglist.PL_DTE.ToString()).ToShortDateString();
            pl.invoice_num = q_packinglist.INVOICE_NUM;
            pl.gross = q_packinglist.GROSS_WEIGHT;

            return View(pl);
        }

        #endregion

        #region "Booking Transaction"


        [ActionName("booking-list")]
        public ActionResult BookingewList(string sc,string s,int? p)
        {

            if (s != null)
            {
                p = 1;
            }
            else
            {
                s = sc;
            }

            ViewBag.CurrentFilter = s;

            List<bookinglist> bklst = new List<bookinglist>();
            TCCGDataContext dbcontext = new TCCGDataContext();

            var q_booklst = from aa in dbcontext.TCCG_BOOKINGs
                            select aa;

            if (!string.IsNullOrEmpty(s))
            {
                q_booklst = q_booklst.Where(aa => aa.booking_num.Contains(s) || aa.invoice_num.Contains(s));
            }

            foreach (var row in q_booklst)
            {
                bklst.Add(new bookinglist { 
                bookingnum=row.booking_num,
                invoicenum=row.invoice_num,
                bill_landing=row.bill_landing,
                vessel=row.vessel_voyage,
                port_landing=row.port_landing,
                port_descharge=row.port_descharge
                });
            }

            

            int pageSize = 10;
            int pageNumber = (p ?? 1);

            return View(bklst.ToPagedList(pageNumber, pageSize));
        }

        [ActionName("new-booking")]
        public ActionResult NewBooking()
        {
            ViewBag.issave = false;
            return View();

        }

        [HttpPost]
        [ActionName("new-booking")]
        public ActionResult SaveBooking(bbooking_setup booksetup)
        {
            TCCGDataContext dbcontext = new TCCGDataContext();

            dbcontext.SPROC_TCCG_BOOKING_SAVE(booksetup.booking_num,
                booksetup.invoice_num,
                booksetup.bill_landing,
                booksetup.shipper,
                booksetup.consignee,
                booksetup.vessel,
                booksetup.port_landing,
                booksetup.port_descharge);
            ViewBag.issave = true;
            return View();
        }

        [ActionName("modify-booking")]
        public ActionResult ModifyBooking(string bk,string invoice)
        {
            TCCGDataContext dbcontext = new TCCGDataContext();

            var q_booking = (from aa in dbcontext.TCCG_BOOKINGs
                            where aa.booking_num == bk && aa.invoice_num == invoice
                            select aa).First();

            bbooking_setup bksetup = new bbooking_setup();
            bksetup.booking_num = bk;
            bksetup.invoice_num = invoice;
            bksetup.bill_landing = q_booking.bill_landing;
            bksetup.shipper = q_booking.shipper_exporter;
            bksetup.consignee = q_booking.consignee;
            bksetup.vessel = q_booking.vessel_voyage;
            bksetup.port_landing = q_booking.port_landing;
            bksetup.port_descharge = q_booking.port_descharge;

            ViewBag.issave = false;
            return View(bksetup);
        }

        [HttpPost]
        [ActionName("modify-booking")]
        public ActionResult ModifyBooking(bbooking_setup booksetup)
        {
            TCCGDataContext dbcontext = new TCCGDataContext();

            dbcontext.SPROC_TCCG_BOOKING_SAVE(booksetup.booking_num,
                booksetup.invoice_num,
                booksetup.bill_landing,
                booksetup.shipper,
                booksetup.consignee,
                booksetup.vessel,
                booksetup.port_landing,
                booksetup.port_descharge);
            ViewBag.issave = true;
            return View();
        }
        #endregion

        #region "Payout Transaction"

        [ActionName("payout-transaction")]
        public ActionResult PayoutTransaction()
        {
            List<partedlist> p_list = new List<partedlist>();
            TCCGDataContext dbcontext = new TCCGDataContext();

            var q_po = from aa in dbcontext.V_PO_PROFITs
                       select aa;

            foreach (var row in q_po)
            {
                p_list.Add(new partedlist { ponum=row.PO_NUM,total_profit=row.TOTAL_PROFIT});
            }
            return View(p_list);
        }

        #endregion
    }
}
