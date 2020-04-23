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
            Expression<Func<Role, bool>> where = item => item.IsDelete==0;

            if (!string.IsNullOrEmpty(RoleNum))
            {
                //当类型不是全部选中项，则按照类型组合条件
                where = where.And(item => item.RoleNum.IndexOf(RoleNum)!=-1 || item.RoleName.IndexOf (RoleNum) != -1);
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

        Rolemanager Rolemanager = new Rolemanager();
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="Information"></param>
        /// <returns></returns>
        public ActionResult AddAjax(string RoleName, string Remark)
        {
            Role role = new Role();
            //获取最大编号
            string roleNum = Rolemanager.GetByWhere(item => item.Id != 1).OrderByDescending(item => item.RoleNum).Take(1).Select(item => item.RoleNum).FirstOrDefault();
            role.RoleNum = "00000" + (int.Parse(roleNum) + 1);

            int num = int.Parse(roleNum);
            if (num >= 9)
            {
                role.RoleNum = "0000" + (int.Parse(roleNum) + 1);
            }
            else if (num >= 99)
            {
                role.RoleNum = "000" + (int.Parse(roleNum) + 1);
            }

            role.RoleName = RoleName;
            role.Remark = Remark;
            role.IsDelete = 0;
            role.CreateTime = DateTime.Now;
            bool val = Rolemanager.Add(role);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ActionResult QueryByRoleId(int roleId)
        {
            Role role = Rolemanager.GetByWhere(item => item.Id == roleId).SingleOrDefault();
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ActionResult Update(string roleNum, string RoleName, string Remark)
        {
            Role role = Rolemanager.GetByWhere(item => item.RoleNum == roleNum).SingleOrDefault();
            role.RoleName = RoleName;
            role.Remark = Remark;
            bool val = Rolemanager.Update(role);
            if (val)
            {
                return Json("修改成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("修改失败", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ActionResult Delete(int roleId)
        {
            Role role = Rolemanager.GetByWhere(item => item.Id == roleId).SingleOrDefault();
            role.IsDelete = 1;
            bool val = Rolemanager.Update(role);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteOther(List<Role> list)
        {
            bool val = true;
            foreach (var item in list)
            {
                Role role = Rolemanager.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                role.IsDelete = 1;
                val = Rolemanager.Update(role);
            }
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
