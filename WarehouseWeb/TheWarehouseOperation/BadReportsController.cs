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
    public class BadReportsController : Controller
    {
        BadReportManager badReport = new BadReportManager();
        BadReportDetailManager badReportDetail = new BadReportDetailManager();
        BadReportTypeManager badReportType = new BadReportTypeManager();
        ProductManager product = new ProductManager();
        /// <summary>
        /// 报损管理
        /// </summary>
        /// <returns></returns>
        // GET: BadReports
        public ActionResult ListBadReport()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string BadNum, string state, string end,int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<BadReport, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate && i.IsDelete == 0;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(BadNum))
            {
                where = where.And(i => i.BadNum.IndexOf(BadNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = badReport.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, BadNum = i.BadNum, BadTypeId = i.BadTypeId, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                BadReportInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(string id)
        {
            Expression<Func<BadReport, bool>> where = i => i.BadNum.IndexOf(id) != -1;
            var b = badReport.GetByWhere(where).SingleOrDefault();
            var d = badReportDetail.GetByWhere(i => i.BadId.IndexOf(id) != -1);
            var t = badReportType.GetByWhere(i => i.Id == b.BadTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = b.Id,
                BadNum = b.BadNum,
                BadTypeId = t.BadTypeName,
                Status = b.Status,
                Num = b.Num,
                AuditUser = b.AuditUser,
                AuditTime = b.AuditTime.ToString("yyyy-MM-dd"),
                Remark = b.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, BadId = i.BadId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Quantity = i.Quantity, Location = i.Location });
            var result = new
            {
                BadReportInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(BadReport i, string status)
        {
            var ss = badReport.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.BadNum = ss.BadNum;
            i.BadTypeId = ss.BadTypeId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.SumMoney = ss.SumMoney;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var badReports = new BadReportManager();
            var s = badReports.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            //报损类型
            var type = badReportType.GetAll();
            type.Insert(0, new BadReportType() { Id = 9999, BadTypeName = "请选择报损类型" });
            ViewBag.BadTypeId = new SelectList(type, "Id", "BadTypeName");
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            return View();
        }

        public ActionResult QueryByProductId(int Id)
        {
            var productInfo = product.GetByWhere(i => i.Id == Id);
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size, OutPrice = item.OutPrice, LocationId = item.Location.LocationName, StockNum = item.StockNum });
            return Json(newFormatList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(List<BadReportDetail> detail, int BadTypeId, string Remark, string AuditUser)
        {
            string detailNum = "";
            //获取明细表最大编号
            string detailNumBig = badReportDetail.GetByWhere(i => true).OrderByDescending(i => i.DetailNum).Take(1).Select(i => i.DetailNum).FirstOrDefault();
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

            string badNum = "";
            //获取出库表最大编号
            string badNumBig = badReport.GetByWhere(i => true).OrderByDescending(i => i.BadNum).Take(1).Select(i => i.BadNum).FirstOrDefault();
            if (badNumBig == null)
            {
                badNum = "000001";
            }
            else
            {
                badNum = "00000" + (int.Parse(badNumBig) + 1);
                int num_2 = int.Parse(badNumBig);
                if (num_2 >= 9)
                {
                    badNum = "0000" + (int.Parse(badNumBig) + 1);
                }
                if (num_2 >= 99)
                {
                    badNum = "000" + (int.Parse(badNumBig) + 1);
                }
            }

            bool val = true;
            string msg = "";
            foreach (var item in detail)
            {
                item.CreateTime = DateTime.Now;
                item.DetailNum = detailNum;
                item.BadId = badNum;
                val = badReportDetail.Add(item);
            }
            if (val)
            {
                var num = badReportDetail.GetByWhere(i => i.BadId == badNum).Sum(i => i.Quantity); //获取总出货数
                var sumMoney = badReportDetail.GetByWhere(i => i.BadId == badNum).Sum(i => i.SumMoney); //获取总价格
                BadReport ost = new BadReport();
                ost.BadNum = badNum;
                ost.BadTypeId = BadTypeId;
                ost.DetailNum = detailNum;
                ost.Num = Convert.ToInt32(num); ;
                ost.SumMoney = Convert.ToInt32(sumMoney);
                ost.Status = "待审核";
                ost.AuditTime = DateTime.Now;
                ost.AuditUser = AuditUser;
                ost.Remark = Remark;
                ost.IsDelete = 0;
                bool val1 = badReport.Add(ost);
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
            Expression<Func<BadReport, bool>> where = i => i.Id == id;
            var s = badReport.GetByWhere(where).SingleOrDefault();
            //报损类型
            var type = badReportType.GetAll();
            type.Insert(0, new BadReportType() { Id = 9999, BadTypeName = "请选择报损类型" });
            ViewBag.BadTypeId = new SelectList(type, "Id", "BadTypeName");
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            return View(s);
        }

        public ActionResult QueryByIdMinXiInfo(int id)
        {
            BadReport ins = badReport.GetByWhere(i => i.Id == id).SingleOrDefault();
            var mx = badReportDetail.GetByWhere(i => i.BadId == ins.BadNum && i.IsDelete == 0);
            return Json(mx, JsonRequestBehavior.AllowGet);
        }

        ////修改出库单
        //public ActionResult UpdtInfo(List<BadReportDetail> detail, int BadTypeId, int customerId, string Remark, string OutSNum)
        //{
        //    //先删除明细
        //    bool val_1 = true;
        //    var outStorageDetails = new OutStorageDetailManager();
        //    var mx = outStorageDetails.GetByWhere(i => i.OutStorageId == OutSNum);
        //    foreach (var item in mx)
        //    {
        //        val_1 = outStorageDetails.Delete(item);
        //    }

        //    //获取明细表最大编号
        //    string detailNumBig = outStorageDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
        //    string detailNum = "00000" + (int.Parse(detailNumBig) + 1);
        //    int num1 = int.Parse(detailNumBig);
        //    if (num1 >= 9)
        //    {
        //        detailNumBig = "0000" + (int.Parse(detailNumBig) + 1);
        //    }
        //    else if (num1 >= 99)
        //    {
        //        detailNumBig = "000" + (int.Parse(detailNumBig) + 1);
        //    }
        //    string msg = "";
        //    bool val = true;
        //    foreach (var item in detail)
        //    {
        //        item.DetailNum = detailNum;
        //        item.OutStorageId = OutSNum;
        //        item.CreateTime = DateTime.Now;
        //        val = outStorageDetail.Add(item);
        //    }
        //    if (val)
        //    {
        //        var num = outStorageDetail.GetByWhere(item => item.OutStorageId == OutSNum).Sum(item => item.Quantity);
        //        var sumMoney = outStorageDetail.GetByWhere(item => item.OutStorageId == OutSNum).Sum(item => item.SumMoney);
        //        var outStorage_1 = new OutStorageManager();
        //        var s = outStorage_1.GetByWhere(i => i.OutSNum == OutSNum).SingleOrDefault();
        //        s.DetailNum = detailNum;
        //        s.OutSTypeId = outSTypeId;
        //        s.CustomerId = customerId;
        //        s.Remark = Remark;
        //        s.Num = Convert.ToInt32(num);
        //        s.SumMoney = Convert.ToInt32(sumMoney);
        //        bool vall = outStorage.Update(s);
        //        if (vall)
        //        {
        //            msg = "修改成功";
        //        }
        //        else
        //        {
        //            msg = "修改失败";
        //        }
        //        msg = "修改成功";
        //    }
        //    else
        //    {
        //        msg = "修改失败";
        //    }
        //    return Json(msg, JsonRequestBehavior.AllowGet);
        //}
    }
}