using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tccgv2.Models;
namespace tccgv2.Controllers
{
    public class PreviewController : Controller
    {
        //
        // GET: /Preview/

        public ActionResult Index()
        {
            return View();
        }

        [ActionName("preview-invoice-report")]
        public ActionResult invoice(string id)
        {
            preview_invoice pre_invoice = new preview_invoice();
            List<preview_invoice_dtl> pre_dtl = new List<preview_invoice_dtl>();

            TCCGDataContext dbcon = new TCCGDataContext();

            var q_invoice = (from aa in dbcon.V_INVOICEs
                            where aa.INVOICE_NUM == id
                            select aa).First();

            var q_invoice_dtl = from aa in dbcon.V_INVOICE_DTLs
                                where aa.DR_num == id
                                select aa;
                               

            pre_invoice.invoice_num = id;
            pre_invoice.invoice_dte = DateTime.Parse(q_invoice.INVOICE_DTE.ToString()).ToShortDateString();
            pre_invoice.total_amt = q_invoice.DR_Total;

            foreach (var row in q_invoice_dtl)
            {
                pre_dtl.Add(new preview_invoice_dtl {
                qty=row.DR_quantity,
                make=row.MAKE,
                model=row.MODEL,
                vin=row.VIN_ENGINE,
                price=row.DR_itemSellingPrice
                });
            }

            pre_invoice.invoice_dtl = pre_dtl;

            return View(pre_invoice);
        }

        [ActionName("packing-list")]
        public ActionResult PackingList(string id)
        {
            TCCGDataContext dbcon = new TCCGDataContext();

            clsRptPackinglist rptpacking = new clsRptPackinglist();
            List<rpt_packinglist_dtl> dtl = new List<rpt_packinglist_dtl>();

            var q_paking = (from aa in dbcon.V_PACKING_LISTs
                           where aa.PL_NUM == id
                           select aa).First();

            var q_dtl = from aa in dbcon.V_PACKING_LIST_DTLs
                        where aa.PL_NUM == id
                        select aa;

            rptpacking.plnum = id;
            rptpacking.pldte = DateTime.Parse(q_paking.PL_DTE.ToString()).ToShortDateString();
            rptpacking.ship = q_paking.DR_Ship;
            rptpacking.via = q_paking.DR_via;
            rptpacking.gross_weight = q_paking.GROSS_WEIGHT;
            rptpacking.customer = q_paking.DR_customer;
            rptpacking.customername = q_paking.CUSTOMER_ADDRESS + "<br/>" + q_paking.CUSTOMER_NAME + " <br/>" + q_paking.cust_mobile;

            foreach (var row in q_dtl)
            {
                dtl.Add(new rpt_packinglist_dtl { qty=row.QTY,make=row.MAKE,model=row.MODEL,vin=row.VIN_ENGINE,net_weight=row.NET_WEIGTH});
            }


            rptpacking.pklist = dtl;

            return View(rptpacking);
        }
    }
}
