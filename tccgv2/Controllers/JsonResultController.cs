using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tccgv2.Models;
namespace tccgv2.Controllers
{
    public class JsonResultController : Controller
    {
        //
        // GET: /JsonResult/

        TCCGDataContext dbcontext = new TCCGDataContext();
        clsprocedure procedure = new clsprocedure();
        Cttg_function_lib.clsfunction clsfunc = new Cttg_function_lib.clsfunction();

        #region "Login"


        public void SetPmsCookie(bool isremember, string username, string password)
        {
            clssecurity sec = new clssecurity();
            if (isremember)
            {
                HttpCookie cookie = Request.Cookies["tccg"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("tccg");
                    cookie["_00un"] = username;
                    cookie["_00up"] = sec.psEncrypt(password);
                    cookie["_00rmd"] = isremember.ToString();
                    Response.Cookies.Add(cookie);

                }
            }
        }

        [ActionName("login")]
        public JsonResult login(TccgLogin login)
        {
            try
            {
                string msg = string.Empty;
                var security = new clssecurity();
                bool? logres;
                bool isresult = false;
                logres = dbcontext.IsLogin(login.uname, security.psEncrypt(login.upass));


                if (logres == true)
                {
                    TCCG_USER up = dbcontext.TCCG_USERs.First(aa => aa.username == login.uname);

                    if (up.userstatus != true)
                    {
                        msg = "Your account is not active, Please contact Administrator";
                    }
                    else
                    {
                        Session["_cid"] = up.username;
                        Session["_crol"] = up.usergroup;
                        Session.Timeout = 30;
                        this.SetPmsCookie(login.RememberMe, login.uname, login.upass);
                        isresult = true;
                    }
                }
                else {
                    msg = "Please Contact Administrator!";
                }

                return Json(new { result = true, msg = msg, isresult = isresult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region "User masterfile"

        [HttpPost]
        [ActionName("edit-user")]
        public JsonResult EditUser(UserSetup details)
        {
            try
            {


                string newpass = dbcontext.TCCG_USERs.Where(aa => aa.username == details.uname).First().userpassword;
                string str_result = string.Empty;
                if (dbcontext.SPROC_SAVE_USER(details.uname, newpass, details.fullname, details.designation,
                    details.email, "SID002", true, DateTime.Now.Date, DateTime.Now.Date).ReturnValue.ToString() == "0")
                {


                    List<TCCG_USER_RIGHT> q_listdel = (from aa in dbcontext.TCCG_USER_RIGHTs
                                                       where aa.Username == details.uname
                                                       select aa).ToList();
                    if (q_listdel.Count != 0)
                    {

                        dbcontext.TCCG_USER_RIGHTs.DeleteAllOnSubmit(q_listdel);
                        dbcontext.SubmitChanges();
                    }

                    foreach (var row in details.accesslst)
                    {

                        if (row.ischeck)
                        {
                            str_result = dbcontext.SPROC_SAVE_USER_RIGHTS(row.menuid, details.uname, procedure.GetUsername()).ReturnValue.ToString();
                        }
                    }

                }

                return Json(new  { result=true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new  { result = false,err=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("create-new-user")]
        public JsonResult CreateNewUser(UserSetup details)
        {
            try
            {
                clssecurity security = new clssecurity();

                string newpass = security.psEncrypt(details.password);
                string str_result = string.Empty;
                if (dbcontext.SPROC_SAVE_USER(details.uname, newpass, details.fullname, details.designation,
                    details.email, "SID002", true, DateTime.Now.Date, DateTime.Now.Date).ReturnValue.ToString() == "0")
                {


                    List<TCCG_USER_RIGHT> q_listdel = (from aa in dbcontext.TCCG_USER_RIGHTs
                                                       where aa.Username == details.uname
                                                       select aa).ToList();
                    if (q_listdel.Count != 0)
                    {

                        dbcontext.TCCG_USER_RIGHTs.DeleteAllOnSubmit(q_listdel);
                        dbcontext.SubmitChanges();
                    }

                    foreach (var row in details.accesslst)
                    {
                        if (row.ischeck)
                        {
                            str_result = dbcontext.SPROC_SAVE_USER_RIGHTS(row.menuid, details.uname, procedure.GetUsername()).ReturnValue.ToString();
                        }
                    }

                }

                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result=false,err=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("delete-user")]
        public JsonResult DelUser(string uname)
        {
            try
            {
                var q_user = from aa in dbcontext.TCCG_USERs
                             where aa.username == uname
                             select aa;

                dbcontext.TCCG_USERs.DeleteAllOnSubmit(q_user);
                dbcontext.SubmitChanges();

                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region "for po transaction"

        [ActionName("save-po")]
        public JsonResult SavePO(clsPOSetup posetup)
        {
            try
            {

                string ponum = string.Empty;

                if (posetup.ponum == "[SYSTEM GENERATED]")
                {
                    ponum = "PO" + DateTime.Now.ToString("yyMMddfff");
                }
                else
                {
                    ponum = posetup.ponum;   
                }


                dbcontext.SPROC_TCCG_PO_MSTR(ponum,
                    DateTime.Parse(posetup.podate),
                    posetup.customercode,
                    posetup.baseprice,
                    posetup.convertedamt,
                    "SID012");

                var q_dtl_del=from aa in dbcontext.TCCG_PO_DTLs
                              where aa.PO_NUM==ponum
                              select aa;
                if(q_dtl_del.Any()){
                    foreach (var row in q_dtl_del)
                    {
                        dbcontext.TCCG_PO_DTLs.DeleteOnSubmit(row);
                    }
                }
                foreach (var row in posetup.podetail)
                {
                    string itm_cde = "ITM" + DateTime.Now.ToString("yyMMddfff");
                    dbcontext.SPROC_TCCG_PO_DTL(ponum,
                        itm_cde,
                        row.qty,
                        row.make,
                        row.model,
                        row.vin_engine,
                        row.baseprice,
                        row.sellingprice,
                        row.actulalamt,
                        row.convertedamt,
                        true);

                }
                dbcontext.SubmitChanges();

                return Json(new {result=true,err=string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result=false,err=ex.Message});
            }
        }

        [ActionName("get-po-details")]
        public JsonResult GetPoDetails(string ponum)
        {
            try
            {
                List<clsPODetails> podetails = new List<clsPODetails>();

                var q_po_master = from aa in dbcontext.TCCG_PO_MSTRs
                                  where aa.PO_NUM == ponum
                                  select aa;

                var q_po_details = from aa in dbcontext.V_PO_DETAILs
                                   where aa.PO_NUM == ponum
                                   select aa;

                foreach (var row in q_po_details)
                {
                    podetails.Add(new clsPODetails {
                    qty=row.QTY,
                    make=row.MAKE,
                    model=row.MODEL,
                    vin_engine=row.VIN_ENGINE,
                    baseprice=row.BASE_PRICE,
                    sellingprice=row.SELLING_PRICE,
                    actulalamt=row.BASE_AMT,
                    convertedamt=row.SELLING_AMT,
                    profit=row.PROFIT
                    });
                }

                return Json(new { result = true,customercode=q_po_master.First().PO_CUSTOMER, podtl = podetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = true, err =ex.Message}, JsonRequestBehavior.AllowGet);
            }
            
        
        }

        #endregion

        #region "Invoice Transaction"

        [ActionName("get-po-num-open")]
        public JsonResult GetPONumOpen(string custcode)
        {
            try
            {
                List<dropdown> ponumlist = new List<dropdown>();

                var q_ponum = from aa in dbcontext.V_PO_LISTs
                              where aa.PO_CUSTOMER == custcode && aa.Description=="unpaid"
                              select new { aa.PO_NO };


                foreach (var row in q_ponum)
                {
                    ponumlist.Add(new dropdown
                    {
                        displayvalue = row.PO_NO,
                        valuemember = row.PO_NO
                    });
                }

                return Json(new { result = true, ponumlist = ponumlist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [ActionName("get-po-details-list")]
        public JsonResult GetPODetails(string ponum)
        {
            try
            {
                List<clsPODetails> podetails = new List<clsPODetails>();

                var q_po_details = from aa in dbcontext.TCCG_PO_DTLs
                                   where aa.PO_NUM == ponum
                                   select aa;

                foreach (var row in q_po_details)
                {
                    podetails.Add(new clsPODetails {itemcode=row.ITMCDE,
                        qty=row.QTY,
                    make=row.MAKE,
                    model=row.MODEL,
                    vin_engine=row.VIN_ENGINE,
                    sellingprice=row.SELLING_PRICE,
                    convertedamt=row.SELLING_AMT});
                }

                return Json(new { result = true, poitem = podetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [ActionName("save-invoice")]
        public JsonResult SaveInvoice(Invoicesave Invoice)
        {
            try
            {
                string invoicenum = string.Empty;
                if (Invoice.Invoice_num == "[SYSTEM GENERATED]")
                {
                    invoicenum = "TCCG" + DateTime.Now.ToString("yyMMddfff");
                }
                else
                {
                    invoicenum = Invoice.Invoice_num;
                }



                string xx = string.Empty;

                xx = dbcontext.SPROC_TCCG_INVOICE_MSTR(invoicenum, Invoice.Invoice_date,
                     Invoice.cust_code, Invoice.ship, Invoice.via, Invoice.Total_amt).ReturnValue.ToString();

                var q_itm_dtl = from aa in dbcontext.TCCG_INVOICE_DTLs
                                where aa.DR_num == Invoice.Invoice_num
                                select aa;

                dbcontext.TCCG_INVOICE_DTLs.DeleteAllOnSubmit(q_itm_dtl);
                dbcontext.SubmitChanges();

                if (xx == "0")
                {
                    foreach (var row in Invoice.itemdetails)
                    {
                        dbcontext.SPROC_TCCG_INVOICE_DTL(invoicenum, row.itmcode, row.ponum, row.qty, row.price, row.amount, 0);
                    }
                }

                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [ActionName("get-invoice-details")]
        public JsonResult GetInvoiceDetails(string id)
        {
            try
            {
                List<Invoice_Details> ind = new List<Invoice_Details>();

                var q_invoice_dtl = from aa in dbcontext.V_INVOICE_DTLs
                                    where aa.DR_num == id
                                    select aa;

                foreach (var row in q_invoice_dtl)
                {
                    ind.Add(new Invoice_Details
                    {
                        refpo = row.ref_po_num,
                        qty = row.DR_quantity,
                        make = row.MAKE,
                        model = row.MODEL,
                        vin = row.VIN_ENGINE,
                        price = row.DR_itemSellingPrice,
                        runingtot = row.DR_total,
                        itmcode = row.item_code
                    });
                }

                return Json(new { result = true, dtl = ind }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("check-invoice")]
        public JsonResult CheckInvoice(string invoicenum)
        {
            try
            {
                bool isreuslt = false;

                var q_invoice = (from aa in dbcontext.V_INVOICEs
                                where aa.INVOICE_NUM == invoicenum
                                select aa).First();


                if (q_invoice.PAID_STATUS != "SID013" || q_invoice.PAID_STATUS != "SID014")
                {
                    isreuslt = true;
                }

                return Json(new { result = true, isreuslt=isreuslt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Payment Transaction"

        
        [ActionName("get-customer-open")]
        public JsonResult GetCustomerOpen()
        {
            try
            {
                List<dropdown> drp = new List<dropdown>();

                var q_opencust = from aa in dbcontext.V_OPEN_CUSTOMERs
                                 select aa;

                drp.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
                foreach (var row in q_opencust)
                {
                    drp.Add(new dropdown {displayvalue=row.CUSTOMER_NAME,valuemember=row.CUSTOMER_CODE });
                }

                return Json(new { result = true, customer = drp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("get-invoice")]
        public JsonResult GetInvoice(string customer)
        {
            try
            {
                List<dropdown> drp = new List<dropdown>();

                var q_invoice_info = from aa in dbcontext.V_OPEN_INVOICEs
                                     where aa.DR_customer == customer
                                     select aa;

                drp.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
                foreach (var row in q_invoice_info)
                {
                    drp.Add(new dropdown { displayvalue = row.DR_num, valuemember = row.DR_num });
                }
                return Json(new { result = true, customer = drp }, JsonRequestBehavior.AllowGet); 
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("get-invoice-dtl")]
        public JsonResult GetInvoicedtl(string invoicenum)
        {
            try
            {
                InvoiceInfo ininfo = new InvoiceInfo();

                var q_invoice_info = (from aa in dbcontext.V_OPEN_INVOICEs
                                     where aa.DR_num == invoicenum
                                     select aa).First();

                ininfo.invoicenum = invoicenum;
                ininfo.totalpayable = q_invoice_info.DR_Balance;

                return Json(new { result = true, ininfo = ininfo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("save-payment")]
        public JsonResult SavePayment(PaymentList paylst)
        {
            try
            {
                string paynum = string.Empty;
                if (paylst._PAY_NUM == "[SYSTEM GENERATED]")
                {
                    paynum = "PY-" + DateTime.Now.ToString("yyMMddfff"); ;
                }
                else
                {
                    paynum = paylst._PAY_NUM;
                }

                dbcontext.SPROC_TCCG_PAYMENT_SAVE(paynum,
                    paylst._INVOICE_NUM,
                   DateTime.Parse(paylst._PAY_DTE),
                    paylst._AMT_PAID,
                    paylst._PAY_BALANCE);

                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("del-payment")]
        public JsonResult DelPayment(string paynum)
        {
            try
            {

                bool isresult=false;

                var q_payinfo = (from aa in dbcontext.V_PAYMENT_LISTs
                                where aa.PAY_NUM == paynum
                                select aa).First();


                var q_invoiceinfo = (from aa in dbcontext.V_INVOICEs
                                     where aa.INVOICE_NUM == q_payinfo.INVOICE_NUM
                                     select aa).First();

                if (q_invoiceinfo.STATUS == "UN POST")
                {
                    isresult = true;
                    dbcontext.SPROC_DELPAYMENT(paynum);
                }


                return Json(new { result = true, isresult=isresult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region "Expense Transaction"

        [ActionName("get-customer-invoice")]
        public JsonResult GetCustomerInvoice()
        {
            try
            {
                List<dropdown> drp = new List<dropdown>();

                var q_opencust = from aa in dbcontext.V_INVOICE_CUSTOMERs
                                 select aa;

                drp.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
                foreach (var row in q_opencust)
                {
                    drp.Add(new dropdown { displayvalue = row.CUSTOMER_NAME, valuemember = row.DR_customer });
                }

                return Json(new { result = true, customer = drp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("get-expense-invoice")]
        public JsonResult GetExpenseInvoice(string customercde)
        {
            try
            {
                List<dropdown> drp = new List<dropdown>();

                var q_opencust = from aa in dbcontext.V_INVOICEs
                                 where aa.DR_customer == customercde
                                 select aa;

                drp.Add(new dropdown { displayvalue = "=Select=", valuemember = "=Select=" });
                foreach (var row in q_opencust)
                {
                    drp.Add(new dropdown { displayvalue = row.INVOICE_NUM, valuemember = row.INVOICE_NUM });
                }

                return Json(new { result = true, invoicenum = drp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("save-expense")]
        public JsonResult SaveExpense(expense_setup ex_setup)
        {
            try
            {
                dbcontext.SPROC_TCCG_EXPENSE_MSTR(ex_setup.invoice_num,
                    ex_setup.customercode,
                    ex_setup.total_amt);

                var q_del_expense = from aa in dbcontext.TCCG_EXPENSE_DTLs
                                    where aa.INVOICE_NUM == ex_setup.invoice_num
                                    select aa;

                dbcontext.TCCG_EXPENSE_DTLs.DeleteAllOnSubmit(q_del_expense);
                dbcontext.SubmitChanges();

                foreach (var row in ex_setup.expensedtl)
                {
                    dbcontext.SPROC_TCCG_EXPENSE_DTL(ex_setup.invoice_num,
                        DateTime.Parse(row.trans_dte.ToString()),
                        row.trans_des,
                        row.trans_amt);
                }

                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("get-expense-details")]
        public JsonResult GetExpenseDetails(string invoice_num)
        {
            try
            {

                expense_setup ex_setup = new expense_setup();
                List<expense_setup_dtl> ex_dtl = new List<expense_setup_dtl>();
                string cusname = string.Empty;

                var q_expense_mstr = (from aa in dbcontext.V_EXPENSEs
                                     where aa.INVOICE_NUM == invoice_num
                                     select aa).First();

                ex_setup.invoice_num = q_expense_mstr.INVOICE_NUM;
                ex_setup.customercode = q_expense_mstr.CUSTOMER_CDE;
                ex_setup.total_amt = q_expense_mstr.EXPENSE_AMT;

                cusname = dbcontext.TCCG_CUSTOMERs.Where(aa => aa.CUSTOMER_CODE == q_expense_mstr.CUSTOMER_CDE).First().CUSTOMER_NAME;

                var q_expense_dtl = from aa in dbcontext.V_EXPENSE_DTLs
                                    where aa.INVOICE_NUM == invoice_num orderby aa.REC_ID ascending
                                    select aa;

                foreach (var row in q_expense_dtl)
                {
                    ex_dtl.Add(new expense_setup_dtl {
                    trans_des=row.TRANS_DES,
                    trans_dte=DateTime.Parse(row.TRANS_DTE.ToString()).ToShortDateString(),
                    trans_amt=row.TRANS_AMT
                    });
                }

                ex_setup.expensedtl = ex_dtl;

                return Json(new { result = true,cusname=cusname, ex_setup = ex_setup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region "Packing List"

        [ActionName("pl-invoice-dtl")]
        public JsonResult PLInvoiceDTL(string invoice_num)
        {
            try
            {

                List<packinglist_dtl> pl_list = new List<packinglist_dtl>();

                var q_invoice_dtl = from aa in dbcontext.V_INVOICE_DTLs
                                    where aa.DR_num == invoice_num
                                    select aa;

                foreach (var row in q_invoice_dtl)
                {
                    pl_list.Add(new packinglist_dtl
                    { 
                    itmcode=row.item_code,
                    qty=row.DR_quantity,
                    make=row.MAKE,
                    model=row.MODEL,
                    vin=row.VIN_ENGINE
                    });
                }

                return Json(new { result = true, pl = pl_list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("pl-save")]
        public JsonResult PLSave( plsv_mstr pl)
        {
            try
            {
                string pl_num = string.Empty;
                if (pl.pl_num == "[SYSTEM GENERATED]")
                {
                    pl_num = "PL" + DateTime.Now.ToString("yyMMddfff");
                }
                else
                {
                    pl_num = pl.pl_num;
                }

                /*-------------SAVE PACKING LIST MASTER-----------------------*/
                dbcontext.SPROC_SAVE_PACKINGLIST_MSTR(pl_num, DateTime.Parse(pl.pl_date), pl.invoice_num, pl.gross);


                /*-------------DELETE PACKING LIST DETAILS--------------------*/
                var q_pck_dtl = from aa in dbcontext.TCCG_PACKING_LIST_DTLs
                                where aa.PL_NUM == pl_num
                                select aa;

                dbcontext.TCCG_PACKING_LIST_DTLs.DeleteAllOnSubmit(q_pck_dtl);
                dbcontext.SubmitChanges();


                /*------------------SAVE PACKING LIST DETAILS---------------------*/

                foreach (var row in pl.dtl)
                {
                    dbcontext.SPROC_SAVE_PACKINGLIST_DTL(pl_num, row.itmcde, row.kg);
                }

                
                return Json(new { result = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [ActionName("pl-dtl")]
        public JsonResult pldtl(string id)
        {
            try
            {
                List<packinglist_dtl> pl_list = new List<packinglist_dtl>();
                var q_dtl = from aa in dbcontext.V_PACKING_LIST_DTLs
                            where aa.PL_NUM == id
                            select aa;

                foreach (var row in q_dtl)
                {
                    pl_list.Add(new packinglist_dtl
                    {
                        itmcode = row.ITEM_CDE,
                        qty = row.QTY,
                        make = row.MAKE,
                        model = row.MODEL,
                        vin = row.VIN_ENGINE,
                        weigth=row.NET_WEIGTH
                    });
                }

                return Json(new { result = true, pl_list=pl_list}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region "Parted Transaction"

        [ActionName("get-person-list")]
        public JsonResult get_person_list(string id)
        {
            try
            {
                List<perted_personlist> person = new List<perted_personlist>();

                var q_person = from aa in dbcontext.V_PAYOUT_LISTs
                               where aa.po_num == id
                               select aa;

                foreach (var row in q_person)
                {
                    person.Add(new perted_personlist
                    {
                        person = row.PersonName,
                        total_rate = row.Total_Rate
                    });
                }

                return Json(new { result = true, q_person = q_person }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion
    }
}
