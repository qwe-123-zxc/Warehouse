using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;

namespace WarehouseWeb.SystemSetup
{
    /// <summary>
    /// 权限分配页面
    /// </summary>
    public class PermissionGrantedController : Controller
    {

        FunctionManager functionManager = new FunctionManager();
        RolePowerManager rolePowerManager = new RolePowerManager();

        
        // GET: PermissionGranted
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowData(int roleId)
        {
            List<Function> list = functionManager.GetByLinqRoleId(roleId);
            List<Function> rootMeun = list.Where(item => item.ParentNodeId == 0 && item.IsDelete == 0).ToList();
            List<Function> list1 = functionManager.GetByLinqRoleIdNot(roleId);
            List<Function> rootMeun1 = list.Where(item => item.ParentNodeId == 0 && item.IsDelete == 0).ToList();
            var result = new
            {
                rightList = list,
                rightListRoot = rootMeun,
                leftList = list1,
                leftListRoot = rootMeun1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}