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
        AdminManager admin = new AdminManager();
        /// <summary>
        /// 报损管理
        /// </summary>
        /// <returns></returns>
        // GET: BadReports
        public ActionResult ListBadReport()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string BadNum, string state, string end,int pageIndex, string UserName)
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
            var adm = admin.GetByWhere(i => i.UserName == UserName).SingleOrDefault();
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, BadNum = i.BadNum, BadTypeId = i.BadReportType.BadTypeName, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd"), audit = adm.RealName });
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
        public ActionResult QueryMinXi(int id)
        {
            Expression<Func<BadReport, bool>> where = i => i.Id == id;
            var b = badReport.GetByWhere(where).SingleOrDefault();
            var d = badReportDetail.GetByWhere(i => i.BadId == b.BadNum);
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
            if (status.Equals("审核通过"))
            {
                var d = badReportDetail.GetByWhere(item => item.BadId == ss.BadNum);
                foreach (var item in d)
                {
                    var pd = new ProductManager();
                    Expression<Func<Product, bool>> where = iss => iss.ProductNum == item.ProductNum;
                    var pdu1 = pd.GetByWhere(where).SingleOrDefault();
                    pdu1.StockNum = Convert.ToInt32(pdu1.StockNum - item.Quantity);
                    var pdu = product.Update(pdu1);
                }
            }
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
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size, OutPrice = item.OutPrice, LocationId = item.LocationId, StockNum = item.StockNum });
            return Json(newFormatList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(List<BadReportDetail> detail, int BadTypeId, string Remark, string AuditUser)
        {
            string detailNum = "";
            //获取明细表最大编号
            string detailNumBig = badReportDetail.GetByWhere(i => true).OrderByDescending(i => i.DetailNum).Take(1).Select(i => i.DetailNum).FirstOrDefault();
            if (!string.IsNullOrEmpty(detailNumBig))
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
                ost.SumMoney = Convert.ToDouble(sumMoney);
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
            ViewBag.BadTypeId = new SelectList(type, "Id", "BadTypeName",s.BadTypeId);
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

        //修改出库单
        public ActionResult UpdtInfo(List<BadReportDetail> detail, int BadTypeId, string Remark, string badSNum)
        {
            //先删除明细
            bool val_1 = true;
            var badReportDetails = new BadReportDetailManager();
            var mx = badReportDetails.GetByWhere(i => i.BadId == badSNum);
            foreach (var item in mx)
            {
                val_1 = badReportDetails.Delete(item);
            }

            //获取明细表最大编号
            string detailNumBig = badReportDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
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
                item.BadId = badSNum;
                val = badReportDetail.Add(item);
            }
            if (val)
            {
                var num = badReportDetail.GetByWhere(item => item.BadId == badSNum).Sum(item => item.Quantity);
                var sumMoney = badReportDetail.GetByWhere(item => item.BadId == badSNum).Sum(item => item.SumMoney);
                var badreport_1 = new BadReportManager();
                var s = badreport_1.GetByWhere(i => i.BadNum == badSNum).SingleOrDefault();
                s.DetailNum = detailNum;
                s.BadTypeId = BadTypeId;
                s.Remark = Remark;
                s.Num = Convert.ToInt32(num);
                s.SumMoney = Convert.ToDouble(sumMoney);
                bool vall = badReport.Update(s);
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
            BadReport ins = badReport.GetByWhere(item => item.Id == id).SingleOrDefault();
            List<BadReportDetail> listDetail = badReportDetail.GetByWhere(item => item.BadId == ins.BadNum);
            bool val = true;
            string msg = "";
            foreach (var list in listDetail)
            {
                list.IsDelete = 1;
                val = badReportDetail.Update(list);
            }
            if (val)
            {
                ins.IsDelete = 1;
                bool vall = badReport.Update(ins);
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

        //全选单选删除
        public ActionResult DeleteOther(List<BadReport> list)
        {
            string msg = "";
            foreach (var item in list)
            {
                BadReport ins = badReport.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                List<BadReportDetail> listDetail = badReportDetail.GetByWhere(i => i.BadId == ins.BadNum);
                bool val = true;
                foreach (var listd in listDetail)
                {
                    listd.IsDelete = 1;
                    val = badReportDetail.Update(listd);
                }
                if (val)
                {
                    ins.IsDelete = 1;
                    bool vall = badReport.Update(ins);
                    if (vall)
                    {
                        msg = "删除成功";
                    }
                    else
                    {
                        msg = "删除失败";
                    }
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}