using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tccgv2.Models;
using PagedList;

namespace tccgv2.Controllers
{
    
    public class MasterFileController : Controller
    {
        //
        // GET: /MasterFile/

        TCCGDataContext dbcontext = new TCCGDataContext();
        clssecurity security = new clssecurity();
        clsprocedure procedure = new clsprocedure();


        public ActionResult Index()
        {
            return View();
        }

        #region "User"
        [ActionName("user-list")]
        public ActionResult UserList(string sc, string s, int? p)
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

            List<Userlist> ulist = new List<Userlist>();
            var dbcontext = new TCCGDataContext();

            var q_userlist = from aa in dbcontext.V_USERs
                         select aa;

            if (!string.IsNullOrEmpty(s))
            {
                q_userlist = q_userlist.Where(aa => aa.USERNAME.Contains(s) || aa.FULLNAME.Contains(s) || aa.DESIGNATION.Contains(s) || aa.EMAIL.Contains(s));
            }

            if (q_userlist.Any())
            {
                foreach (var row in q_userlist)
                {
                    ulist.Add(new Userlist { username=row.USERNAME,fullname=row.FULLNAME,email=row.EMAIL,designation=row.DESIGNATION,Group=row.GROUPDES,isactive=row.ISACTIVE.ToString()});
                }
            }

            int pageSize = 10;
            int pageNumber = (p ?? 1);

            return View(ulist.ToPagedList(pageNumber, pageSize));
        }

        [ActionName("create-new-user")]
        public ActionResult CreateNewUser()
        {

            UserSetup usetup = new UserSetup();
            List<AccessList> acclist = new List<AccessList>();
            var q_menulist = from aa in dbcontext.TCCG_MENUs
                             where aa.ParentMenuID != "0" orderby aa.MenuOrder
                             select aa;
            if (q_menulist.Any())
            {
                foreach (var row in q_menulist)
                {
                    acclist.Add(new AccessList { menuid=row.MenuID,menuname=row.MenuText});
                }
            }

            usetup.accesslst = acclist;

            return View(usetup);
        }

   

       

        [ActionName("edit-user")]
        public ActionResult EditUser(string id)
        {
            UserSetup usetup = new UserSetup();
            List<AccessList> acclist = new List<AccessList>();

            var q_userprofile = from aa in dbcontext.TCCG_USERs
                                where aa.username == id
                                select aa;

            var q_usermenu = from aa in dbcontext.TCCG_USER_RIGHTs
                             where aa.Username == id
                             select aa;

            if (q_userprofile.Any())
            {
                usetup.uname = id;
                usetup.designation = q_userprofile.First().userdesignation;
                usetup.email = q_userprofile.First().useremail;
                usetup.fullname = q_userprofile.First().userfullname;
                
            }

            var q_menulist = from aa in dbcontext.TCCG_MENUs
                             where aa.ParentMenuID != "0"
                             orderby aa.MenuOrder
                             select aa;

            if (q_menulist.Any())
            {
                foreach (var row in q_menulist)
                {
                    bool hasmenu = false;

                    foreach (var umenu in q_usermenu)
                    {
                        if (row.MenuID == umenu.MenuID)
                        {
                            hasmenu = true;
                        }
                    }

                    acclist.Add(new AccessList { menuid = row.MenuID, menuname = row.MenuText, ischeck = hasmenu });
                }
            }

            usetup.accesslst = acclist;
            return View(usetup);
        }

        #endregion

        #region "Client"

        [ActionName("client-inquiry")]
        public ActionResult ClientInquiry(int? page)
        {
            List<clsClientList> cllist = new List<clsClientList>();

            var q_clientlist = from aa in dbcontext.TCCG_CLIENTs
                               select aa;

            if (q_clientlist.Any())
            {
                foreach (var row in q_clientlist)
                {
                    cllist.Add(new clsClientList {client_id=row.CLIENT_ID.ToString(),
                    name=row.CLIENT_NAME,
                    address=row.CLIENT_ADDRESS,
                    tel=row.CLIENT_TEL,
                    fax=row.CLIENT_FAX,
                    mobile=row.CLIENT_MOBILE,
                    iscustomer=row.ISCUSTOMER.ToString(),
                    inquirydate=DateTime.Parse(row.INQUIRY_DATE.ToString()).ToShortDateString()
                    });
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(cllist.ToPagedList(pageNumber, pageSize));
            
        }

        [ActionName("add-new-client")]
        public ActionResult CreateNewClientInquiry()
        {
            return View();
        }

        [HttpPost]
        [ActionName("add-new-client")]
        public ActionResult CreateNewClientInquiry(clsClientSetup cls)
        {
            dbcontext.SPROC_SAVE_CLIENT(0, cls.CLIENT_NAME, cls.CLIENT_ADDRESS, cls.CLIENT_TEL,
                cls.CLIENT_FAX, cls.CLIENT_MOBILE, cls.CLIENT_EMAIL, cls.INQUIRY_DETAILS,cls.INQUIRY_DATE, cls.DISPOSITION, cls.REMARKS,
                DateTime.Now.Date, cls.ISCUSTOMER, "SID005");

            return View();
        }

        [ActionName("edit-client")]
        public ActionResult EditClient(string id)
        {
            clsClientSetup client = new clsClientSetup();

            var q_client = (from aa in dbcontext.TCCG_CLIENTs
                           where aa.CLIENT_ID == int.Parse(id)
                           select aa).First();

            client.CLIENT_NAME = q_client.CLIENT_NAME;
            client.CLIENT_ADDRESS = q_client.CLIENT_ADDRESS;
            client.CLIENT_TEL = q_client.CLIENT_TEL;
            client.CLIENT_FAX = q_client.CLIENT_FAX;
            client.CLIENT_MOBILE = q_client.CLIENT_MOBILE;
            client.CLIENT_EMAIL = q_client.CLIENT_EMAIL;
            client.INQUIRY_DETAILS = q_client.INQUIRY_DETAILS;
            client.INQUIRY_DATE = q_client.INQUIRY_DATE;
            client.DISPOSITION = q_client.DISPOSITION;
            client.REMARKS = q_client.REMARKS;
            client.ISCUSTOMER = q_client.ISCUSTOMER;

            return View(client);
        }


        #endregion

        #region "Customer"

        [ActionName("customer-list")]
        public ActionResult CustomerList(int? page)
        {
            List<clsCustomerList> cllist = new List<clsCustomerList>();

            var q_clientlist = from aa in dbcontext.TCCG_CLIENTs
                               where aa.ISCUSTOMER==true
                               select aa;

            if (q_clientlist.Any())
            {
                foreach (var row in q_clientlist)
                {
                    cllist.Add(new clsCustomerList
                    {
                        client_id = row.CLIENT_ID.ToString(),
                        name = row.CLIENT_NAME,
                        address = row.CLIENT_ADDRESS,
                        tel = row.CLIENT_TEL,
                        fax = row.CLIENT_FAX,
                        mobile = row.CLIENT_MOBILE,
                        inquirydate = DateTime.Parse(row.INQUIRY_DATE.ToString()).ToShortDateString()
                    });
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(cllist.ToPagedList(pageNumber, pageSize));

        }

        #endregion
    }
}
