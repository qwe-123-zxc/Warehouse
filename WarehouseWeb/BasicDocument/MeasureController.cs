using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Linq.Expressions;
using WarehouseBLL;
namespace WarehouseWeb.BasicDocument
{
    /// <summary>
    /// 计量单位页面
    /// </summary>
    public class MeasureController : Controller
    {
        // GET: Measure
        public ActionResult Index()
        {
            return View();
        }
        public int PageSize
        {
            get { return 5; }
        }
        MeasureManage service = new MeasureManage();
        public ActionResult Query(string MeasureNum, int pageIndex)
        {
            Expression<Func<Measure, bool>> where = item => true;
            if (!string.IsNullOrEmpty(MeasureNum))
            {
                where = where.And(item => item.MeasureNum.IndexOf(MeasureNum) != -1 || item.MeasureName.IndexOf(MeasureNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, MeasureNum = item.MeasureNum, MeasureName = item.MeasureName,  CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Insert(string measureName)
        {
            Measure measure = new Measure();
            //获取最大编号
            string MeasureNum = service.GetByWhere(item => true).OrderByDescending(item => item.MeasureNum).Take(1).Select(item => item.MeasureNum).FirstOrDefault();
            measure.MeasureNum = "00000" + (int.Parse(MeasureNum) + 1);
            measure.MeasureName = measureName;
            measure.IsDelete = 0;
            measure.CreateTime = DateTime.Now;
            measure.CreateUser = "DA_0000";
            bool val = service.Add(measure);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(string measureNum)
        {
            Measure measure = service.GetByWhere(item => item.MeasureNum.IndexOf(measureNum) != -1).SingleOrDefault();
            bool val = service.Delete(measure);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult QueryById(string measureNum)
        {
            Measure measure = service.GetByWhere(item => item.MeasureNum.IndexOf(measureNum) != -1).SingleOrDefault();
            return Json(measure, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string measureName,string measureNum)
        {
            Measure measure = service.GetByWhere(item => item.MeasureNum.IndexOf(measureNum) != -1).SingleOrDefault();
            measure.MeasureName = measureName;
            bool val = service.Update(measure);
            if (val)
            {
                return Json("修改成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("修改失败", JsonRequestBehavior.AllowGet);
            }
        }
    }
}