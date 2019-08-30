using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tccgv2.Models
{
    public class clsMenu
    {
        public string menuname { get; set; }
        public string menuvalue { get; set; }
        public string menucontroller { get; set; }
        public List<clsSubMenu> submenulist { get; set; }
    }

    public class clsSubMenu
    {
        public string submenuid { get; set; }
        public string submenuname { get; set; }
        public string subcontroller { get; set; }
    }
}