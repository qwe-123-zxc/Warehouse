using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;

namespace WarehouseWeb.SystemSetup
{
    /// <summary>
    /// 角色管理页面
    /// </summary>
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 每页显示2条
        /// </summary>
        public int PageSize
        {
            get { return 2; }
        }
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRole(string RoleNum, int pageIndex)
        {
            RoleManeger service = new RoleManeger();
            //组合条件
            Expression<Func<Role, bool>> where = item => true;

            if (!string.IsNullOrEmpty(RoleNum))
            {
                //当类型不是全部选中项，则按照类型组合条件
                where = where.And(item => item.RoleNum.Equals(RoleNum) || item.RoleName == RoleNum);
            }

            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);
            //返回数据
            //Actionresult  常用响应类型  ViewResult ContentResult JsonResult
            // Json数据格式 { 名称:值 } 数组 [{},{}]
            // 格式转换
            var newFormatList = list.Select(item => new { Id = item.Id, RoleNum = item.RoleNum, RoleName = item.RoleName, Remark = item.Remark, IsDelete = item.IsDelete, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

            //将数据构建打包给前台
            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}