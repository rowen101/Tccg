using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using tccgv2.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;


namespace tccgv2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session.Count == 0)
            {
                return RedirectToAction("Login");
               
            }
            else{
                return View();
            }

          
        }

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

        public bool isRemenber()
        {
            bool isresult = false;
            HttpCookie cookie = Request.Cookies["tccg"];

            if (cookie != null)
            {
                isresult = bool.Parse(cookie["_00rmd"].ToString());
            }


            return isresult;
        }

        public void CreateNewSession()
        {
            var dbcontext = new TCCGDataContext();

            HttpCookie cookie = Request.Cookies["tccg"];
            string uname = string.Empty, upass = string.Empty;

            if (cookie != null)
            {
                uname = cookie["_00un"].ToString();
                upass = cookie["_00up"].ToString();
            }

            TCCG_USER up = dbcontext.TCCG_USERs.First(aa => aa.username == uname);
            Session["_cid"] = up.username;
            Session["_crol"] = up.usergroup;
            Session.Timeout = 30;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            @ViewBag.Title = "Admin | Login";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(TccgLogin login)
        {
            var dbcontext = new TCCGDataContext();
            var security = new clssecurity();
            bool logres;
            bool loginstatus = true;
            string strerrormsg = string.Empty;

            if (string.IsNullOrEmpty(login.uname) || string.IsNullOrEmpty(login.upass))
            {
                @ViewBag.Title = "Admin | Login";
                return View();
            }


            logres = dbcontext.IsLogin(login.uname.ToUpper(), security.psEncrypt(login.upass)) == true ? true : false;


            if (logres)
            {
                 TCCG_USER up = dbcontext.TCCG_USERs.First(aa => aa.username == login.uname );

                if (up.userstatus != true)
                {
                    strerrormsg += "Sorry your account is inactive!";
                    loginstatus = false;
                }


                if (!loginstatus)
                {
                    ViewBag.error = strerrormsg;
                    @ViewBag.Title = "TCCG | Login";
                }
                else
                {
                    Session["_cid"] = up.username;
                    Session["_crol"] = up.usergroup;
                    Session.Timeout = 30;
                }
            }


            if (!loginstatus)
            {
                return View();
            }
            else
            {
                this.SetPmsCookie(login.RememberMe, login.uname.ToUpper(), login.upass);

                return RedirectToAction("Index");
            }


        }

        public ActionResult MenuList()
        {
            bool islogin = false;
            List<clsMenu> mnulst = new List<clsMenu>();
           

            //if the session is count=0 then check the isremember function is equal to true the createnewsession
            if (Session.Count == 0)
            {
                if (isRemenber())
                {
                    this.CreateNewSession();
                }
                else
                {
                    islogin = true;
                }

            }

            if (Session.Count > 0)
            {
                string userid = (string)Session["_cid"];

                var dbcontext = new  TCCGDataContext();


                var q_prntmenu = from aa in dbcontext.V_PARENT_MENUs
                                 where aa.Username == userid orderby aa.MenuOrder
                                 select aa;

                if (q_prntmenu.Any())
                {
                    foreach (var row in q_prntmenu)
                    {
                        List<clsSubMenu> submenu = new List<clsSubMenu>();

                        var q_menu = from aa in dbcontext.V_MENUs
                                     where aa.Username == userid && aa.ParentMenuID==row.MenuID orderby aa.MenuOrder
                                     select aa;

                        if(q_menu.Any())
                        {
                            foreach (var rowmenu in q_menu)
                            {
                                submenu.Add(new clsSubMenu { submenuid=rowmenu.MenuID,submenuname=rowmenu.MenuText,subcontroller=rowmenu.CONTROLER_NAME});
                            }
                        }

                        mnulst.Add(new clsMenu { menuname=row.MenuText,menuvalue=row.MenuID,menucontroller=row.CONTROLER_NAME,submenulist=submenu});
                                   
                    }
                }
            }

            if (islogin)
            {
                return Content("NotLogin");
            }
            else
            {
                return PartialView("_menulist", mnulst);
            }


        }


        private void resizeImage(string path, string originalFilename,
                     int canvasWidth, int canvasHeight,
                     int originalWidth, int originalHeight)
        {
            Image image = Image.FromFile(path + originalFilename);

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                             100L);
            thumbnail.Save(path   + "." + originalFilename, info[1],encoderParameters);
        }

        public ActionResult FileUpload(HttpPostedFileBase[] files)
        {
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/Images/profile"), pic);
                    string path2 = System.IO.Path.Combine(
                                           Server.MapPath("~/Images/profile"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    //System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                    Image image = Image.FromFile(path);
                    Image thumb = image.GetThumbnailImage(140, 140, () => false, IntPtr.Zero);
                    thumb.Save(Path.ChangeExtension(path2, ".jpg"));

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }
            }
            // after successfully uploading redirect the user
            return RedirectToAction("show-uploaded-images");
        }

        [ActionName("show-uploaded-images")]
        public ActionResult ShowUploadImg()
        {
            List<clsImagelist> imglst = new List<clsImagelist>();
            string path = Server.MapPath("~/Images/thumbnails");
            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
            {
                string imgfile = Path.GetFileName(fileName);
                imglst.Add(new clsImagelist { imgpath="~/Images/profile/" + imgfile});
            }
                


            return View(imglst);
        }

        [ActionName("show-api")]
        public ActionResult show_api()
        {
            var json = new WebClient().DownloadString("http://150.200.30.115/food_api/get_categories.php");

            return Content("");
        }

        public ActionResult logout()
        {
            Session.RemoveAll();
            HttpCookie cookie = new HttpCookie("tccg");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login");
         
        }
    }
}
