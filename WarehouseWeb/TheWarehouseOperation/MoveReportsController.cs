using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class MoveReportsController : Controller
    {
        MoveReportManager moveReport = new MoveReportManager();
        MoveReportDetailManager moveReportDetail = new MoveReportDetailManager();
        MoveReportTypeManager moveReportType = new MoveReportTypeManager();
        ProductManager product = new ProductManager();
        LocationManager location = new LocationManager();
        /// <summary>
        /// 移库管理
        /// </summary>
        /// <returns></returns>
        // GET: MoveReports
        public ActionResult ListMoveReport()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string MoveNum, string state, string end, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<MoveReport, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate && i.IsDelete == 0;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(MoveNum))
            {
                where = where.And(i => i.MoveNum.IndexOf(MoveNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = moveReport.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, MoveNum = i.MoveNum, MoveTypeId = i.MoveReportType.MoveTypeName, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                MoveReportInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(int id)
        {
            Expression<Func<MoveReport, bool>> where = i => i.Id == id; 
            var s = moveReport.GetByWhere(where).SingleOrDefault();
            var d = moveReportDetail.GetByWhere(i => i.MoveId == s.MoveNum);
            var t = moveReportType.GetByWhere(i => i.Id == s.MoveTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                MoveNum = s.MoveNum,
                MoveTypeId = t.MoveTypeName,
                Status = s.Status,
                Num = s.Num,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, MoveId = i.MoveId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Quantity = i.Quantity, TheCurrentLocation = i.TheCurrentLocation, MovingLocation = i.MovingLocation });
            var result = new
            {
                MoveReportInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(MoveReport i, string status)
        {
            var ss = moveReport.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.MoveNum = ss.MoveNum;
            i.MoveTypeId = ss.MoveTypeId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var moveReports = new MoveReportManager();
            var s = moveReports.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            //移库类型
            var type = moveReportType.GetAll();
            type.Insert(0, new MoveReportType() { Id = 9999, MoveTypeName = "请选择报损类型" });
            ViewBag.MoveTypeId = new SelectList(type, "Id", "MoveTypeName");
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            //库位
            var location_1 = location.GetAll();
            location_1.Insert(0, new Location() { Id = 9999, LocationName = "请选择产品" });
            ViewBag.Location = new SelectList(location_1, "Id", "LocationName");
            return View();
        }

        public ActionResult QueryByProductId(int Id)
        {
            var productInfo = product.GetByWhere(i => i.Id == Id);
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size,LocationId = item.LocationId, StockNum = item.StockNum});
            return Json(newFormatList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(List<MoveReportDetail> detail, int MoveTypeId, string Remark, string AuditUser)
        {
            string detailNum = "";
            //获取明细表最大编号
            string detailNumBig = moveReportDetail.GetByWhere(i => true).OrderByDescending(i => i.DetailNum).Take(1).Select(i => i.DetailNum).FirstOrDefault();
            if (detailNumBig == null)
            {
                detailNum = "000001";
            }
            else
            {
                detailNum = "00000" + (int.Parse(detailNumBig) + 1);
                int num_1 = int.Parse(detailNumBig);
                if (num_1 >= 9)
                {
                    detailNum = "0000" + (int.Parse(detailNumBig) + 1);
                }
                if (num_1 >= 99)
                {
                    detailNum = "000" + (int.Parse(detailNumBig) + 1);
                }
            }

            string moveNum = "";
            //获取出库表最大编号
            string moveNumBig = moveReport.GetByWhere(i => true).OrderByDescending(i => i.MoveNum).Take(1).Select(i => i.MoveNum).FirstOrDefault();
            if (moveNumBig == null)
            {
                moveNum = "000001";
            }
            else
            {
                moveNum = "00000" + (int.Parse(moveNumBig) + 1);
                int num_2 = int.Parse(moveNumBig);
                if (num_2 >= 9)
                {
                    moveNum = "0000" + (int.Parse(moveNumBig) + 1);
                }
                if (num_2 >= 99)
                {
                    moveNum = "000" + (int.Parse(moveNumBig) + 1);
                }
            }

            bool val = true;
            string msg = "";
            foreach (var item in detail)
            {
                item.CreateTime = DateTime.Now;
                item.DetailNum = detailNum;
                item.MoveId = moveNum;
                val = moveReportDetail.Add(item);
            }
            if (val)
            {
                var num = moveReportDetail.GetByWhere(i => i.MoveId == moveNum).Sum(i => i.Quantity); //获取总出货数
                MoveReport mov = new MoveReport();
                mov.MoveNum = moveNum;
                mov.MoveTypeId = MoveTypeId;
                mov.DetailNum = detailNum;
                mov.Num = Convert.ToInt32(num); ;
                mov.Status = "待审核";
                mov.AuditTime = DateTime.Now;
                mov.AuditUser = AuditUser;
                mov.Remark = Remark;
                mov.IsDelete = 0;
                bool val1 = moveReport.Add(mov);
                if (val1)
                {
                    msg = "新增成功";
                }
                else
                {
                    msg = "新增失败";
                }
                msg = "新增成功";
            }
            else
            {
                msg = "新增失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult UpdtList(int id)
        {
            Expression<Func<MoveReport, bool>> where = i => i.Id == id;
            var s = moveReport.GetByWhere(where).SingleOrDefault();

            //移库类型
            var type = moveReportType.GetAll();
            type.Insert(0, new MoveReportType() { Id = 9999, MoveTypeName = "请选择报损类型" });
            ViewBag.MoveTypeId = new SelectList(type, "Id", "MoveTypeName",s.MoveTypeId);
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            //库位
            var location_1 = location.GetAll();
            location_1.Insert(0, new Location() { Id = 9999, LocationName = "请选择产品" });
            ViewBag.Location = new SelectList(location_1, "Id", "LocationName");
            return View(s);
        }

        public ActionResult QueryByIdMinXiInfo(int id)
        {
            MoveReport ins = moveReport.GetByWhere(i => i.Id == id).SingleOrDefault();
            var mx = moveReportDetail.GetByWhere(i => i.MoveId == ins.MoveNum && i.IsDelete == 0);
            return Json(mx, JsonRequestBehavior.AllowGet);
        }

        //修改出库单
        public ActionResult UpdtInfo(List<MoveReportDetail> detail, int MoveTypeId, string Remark, string movSNum)
        {
            //先删除明细
            bool val_1 = true;
            var moveReportDetails = new MoveReportDetailManager();
            var mx = moveReportDetails.GetByWhere(i => i.MoveId == movSNum);
            foreach (var item in mx)
            {
                val_1 = moveReportDetails.Delete(item);
            }

            //获取明细表最大编号
            string detailNumBig = moveReportDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
            string detailNum = "";
            if (detailNumBig == null)
            {
                detailNumBig = "000001";
            }
            else
            {
                detailNum = "00000" + (int.Parse(detailNumBig) + 1);
                int num1 = int.Parse(detailNumBig);
                if (num1 >= 9)
                {
                    detailNumBig = "0000" + (int.Parse(detailNumBig) + 1);
                }
                else if (num1 >= 99)
                {
                    detailNumBig = "000" + (int.Parse(detailNumBig) + 1);
                }
            }
            string msg = "";
            bool val = true;
            foreach (var item in detail)
            {
                item.DetailNum = detailNum;
                item.CreateTime = DateTime.Now;
                item.MoveId = movSNum;
                val = moveReportDetail.Add(item);
            }
            if (val)
            {
                var num = moveReportDetail.GetByWhere(item => item.MoveId == movSNum).Sum(item => item.Quantity);
                var moveReport_1 = new MoveReportManager();
                var s = moveReport_1.GetByWhere(i => i.MoveNum == movSNum).SingleOrDefault();
                s.DetailNum = detailNum;
                s.MoveTypeId = MoveTypeId;
                s.Remark = Remark;
                s.Num = Convert.ToInt32(num);
                bool vall = moveReport.Update(s);
                if (vall)
                {
                    msg = "修改成功";
                }
                else
                {
                    msg = "修改失败";
                }
                msg = "修改成功";
            }
            else
            {
                msg = "修改失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        //删除报损单
        public ActionResult DeleteInfo(int id)
        {
            MoveReport ins = moveReport.GetByWhere(item => item.Id == id).SingleOrDefault();
            List<MoveReportDetail> listDetail = moveReportDetail.GetByWhere(item => item.MoveId == ins.MoveNum);
            bool val = true;
            string msg = "";
            foreach (var list in listDetail)
            {
                list.IsDelete = 1;
                val = moveReportDetail.Update(list);
            }
            if (val)
            {
                ins.IsDelete = 1;
                bool vall = moveReport.Update(ins);
                if (vall)
                {
                    msg = "删除成功";
                }
                else
                {
                    msg = "删除失败";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}