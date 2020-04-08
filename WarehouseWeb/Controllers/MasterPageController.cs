using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;

namespace WarehouseWeb.Controllers
{
    public class MasterPageController : Controller
    {
        // GET: MasterPage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        FunctionManager manager = new FunctionManager();

        public ActionResult GetSidebarData()
        {
            List<Function> list = manager.GetAll();
            List<Function> rootMeun = list.Where(item => item.ParentNodeId == 0 && item.IsDelete == 0).ToList();
            var newList = new
            {
                list = list,
                rootMeun = rootMeun
            };
            return Json(newList, JsonRequestBehavior.AllowGet);
        }
    }
}