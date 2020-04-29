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
    public class InStoragesController : System.Web.Mvc.Controller
    {
        SupplierManager GonYinShang = new SupplierManager();    //供应商
        InStorageTypeManager inStorageType = new InStorageTypeManager();    //入库类型
        InStorageManager inStorage = new InStorageManager();    //入库管理
        InStorageDetailManager inStorageDetail = new InStorageDetailManager(); //入库明细
        ProductManager productManager = new ProductManager();//产品表
        LocationManager locationManager = new LocationManager();//库位表
        AdminManager admin = new AdminManager();

        /// <summary>
        /// 入库管理
        /// </summary>
        /// <returns></returns>
        // GET: InStorages
        public ActionResult List()
        {
            //供应商
            var gys = GonYinShang.GetAll();
            gys.Insert(0, new Supplier() { Id = 99999999, SupplierName = "请选择供应商" });
            ViewBag.SupplierId = new SelectList(gys, "Id", "SupplierName");

            //单据类型
            var lty = inStorageType.GetAll();
            lty.Insert(0, new InStorageType() { Id = 9999, InSTypeName = "请选择入库单类型" });
            ViewBag.InSTypeId = new SelectList(lty, "Id", "InSTypeName");

            return View();
        }

        public ActionResult ListAjax(string zt, string InSNum, string state, string end, int SupplierId, int InSTypeId, int pageIndex,string UserName)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<InStorage, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate && i.IsDelete == 0;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(InSNum))
            {
                where = where.And(i => i.InSNum.IndexOf(InSNum) != -1);
            }
            if (SupplierId != 99999999)
            {
                where = where.And(i => i.SupplierId == SupplierId);
            }
            if (InSTypeId != 9999)
            {
                where = where.And(i => i.InSTypeId == InSTypeId);
            }
            var pageCount = 0;
            var count = 0;
            var s = inStorage.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            var adm = admin.GetByWhere(i => i.UserName == UserName).SingleOrDefault();
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, InSNum = i.InSNum, InSTypeId = i.InStorageType.InSTypeName, SupplierId = i.Supplier.SupplierName, Num = i.Num, SumMoney = i.SumMoney, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd"), audit = adm.RealName });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                InstorageInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueryMinXi(int id)
        {
            Expression<Func<InStorage, bool>> where = i => i.Id == id;
            var s = inStorage.GetByWhere(where).SingleOrDefault();
            var d = inStorageDetail.GetByWhere(i => i.InStorageId == s.InSNum && i.IsReturnOrder == 0);
            var t = inStorageType.GetByWhere(i => i.Id == s.InSTypeId).SingleOrDefault();
            var g = GonYinShang.GetByWhere(i => i.Id == s.SupplierId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                rukudanhao = s.InSNum,
                rukuleixin = t.InSTypeName,
                zt = s.Status,
                gonyinshangId = g.SupplierNum,
                gonyinshangName = g.SupplierName,
                lianxiren = g.Contacts,
                chuanjianren = s.AuditUser,
                chuanjianTime = s.AuditTime.ToString("yyyy-MM-dd"),
                phone = g.Phone,
                beizhu = s.Remark
            };
            //明细
            var dd = d.Select(i=>new { Id=i.Id, DetailNum=i.DetailNum, InStorageId=i.InStorageId, ProductNum=i.ProductNum, ProductName=i.ProductName, Size=i.Size, UnitPrice=i.UnitPrice, Quantity=i.Quantity, SumMoney=i.SumMoney, Location=i.Location });
            var result = new
            {
                InstorageInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(InStorage i,string status)
        {
            var ss = inStorage.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.InSNum = ss.InSNum;
            i.InSTypeId = ss.InSTypeId;
            i.SupplierId = ss.SupplierId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.SumMoney = ss.SumMoney;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var inStorages = new InStorageManager();
            var s = inStorages.Update(i);
            if (status.Equals("审核通过"))
            {
                var d = inStorageDetail.GetByWhere(item => item.InStorageId == ss.InSNum);
                foreach (var item in d)
                {
                    var pd = new ProductManager();
                    Expression<Func<Product, bool>> where = iss => iss.ProductNum == item.ProductNum;
                    var pdu1 = pd.GetByWhere(where).SingleOrDefault();
                    pdu1.StockNum = Convert.ToInt32(pdu1.StockNum + item.Quantity);
                    var pdu = productManager.Update(pdu1);
                }
            }
            var result = new
            {
                ActionResult = s
            };
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        //新增页面
        public ActionResult ListAdd()
        {
            //供应商
            var gys = GonYinShang.GetAll();
            gys.Insert(0, new Supplier() { Id = 99999999, SupplierName = "请选择供应商" });
            ViewBag.SupplierId = new SelectList(gys, "Id", "SupplierName");
            //单据类型
            var lty = inStorageType.GetAll();
            lty.Insert(0, new InStorageType() { Id = 9999, InSTypeName = "请选择入库单类型" });
            ViewBag.InSTypeId = new SelectList(lty, "Id", "InSTypeName");
            //单据类型
            var product = productManager.GetAll();
            product.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product, "Id", "ProductName");
            //库位类型
            var location = locationManager.GetAll();
            location.Insert(0, new Location() { Id = 9999, LocationName = "请选择库位" });
            ViewBag.location = new SelectList(location, "Id", "LocationName");
            return View();
        }

        //根据产品ID进行查询
        public ActionResult QueryByProductId(int Id)
        {
            var productInfo = productManager.GetByWhere(i => i.Id == Id);
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size, OutPrice = item.OutPrice });
            return Json(newFormatList, JsonRequestBehavior.AllowGet);
        }

        //新增入库单
        public ActionResult Insert(List<InStorageDetail> detail,int InSTypeId,int SupplierId,string Remark,string AuditUser)
        {
            //获取明细表最大编号
            string detailNumBig = inStorageDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
            string detailNum = "";
            if (detailNumBig==null)
            {
                detailNum = "000001";
            }
            else
            { 
                detailNum = "00000" + (int.Parse(detailNumBig) + 1);
                int numD = int.Parse(detailNumBig);
                if (numD >= 9)
                {
                    detailNum = "0000" + (int.Parse(detailNumBig) + 1);
                }
                else if (numD >= 99)
                {
                    detailNum = "000" + (int.Parse(detailNumBig) + 1);
                }
            }
            //获取入库表最大编号
            string inSNumBig = inStorage.GetByWhere(item => true).OrderByDescending(item => item.InSNum).Take(1).Select(item => item.InSNum).FirstOrDefault();
            string inSNum = "";
            if (inSNumBig == null)
            {
                inSNum = "000001";
            }
            else
            {
                inSNum = "00000" + (int.Parse(inSNumBig) + 1);
                int numS = int.Parse(inSNumBig);
                if (numS >= 9)
                {
                    inSNum = "0000" + (int.Parse(inSNumBig) + 1);
                }
                else if (numS >= 99)
                {
                    inSNum = "000" + (int.Parse(inSNumBig) + 1);
                }
            }
            bool val = true ;
            string msg = "";
            foreach (var item in detail)
            {
                item.DetailNum = detailNum;
                item.InStorageId = inSNum;
                item.CreateTime = DateTime.Now;
                val = inStorageDetail.Add(item);
            }
            if (val)
            {
                var num = inStorageDetail.GetByWhere(item => item.InStorageId==inSNum).Sum(item=>item.Quantity);
                var sumMoney = inStorageDetail.GetByWhere(item => item.InStorageId == inSNum).Sum(item => item.SumMoney);
                InStorage inStorages = new InStorage();
                inStorages.AuditTime = DateTime.Now;
                inStorages.AuditUser = AuditUser;
                inStorages.DetailNum = detailNum;
                inStorages.InSNum = inSNum;
                inStorages.InSTypeId = InSTypeId;
                inStorages.IsDelete = 0;
                inStorages.Num = Convert.ToInt32(num); 
                inStorages.Remark = Remark;
                inStorages.Status = "待审核";
                inStorages.SumMoney = Convert.ToDouble(sumMoney); 
                inStorages.SupplierId = SupplierId;
                bool vall = inStorage.Add(inStorages);
                if (vall)
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

        //修改页面
        public ActionResult UpdtList(int id)
        {
            Expression<Func<InStorage, bool>> where = i => i.Id == id;
            var s = inStorage.GetByWhere(where).SingleOrDefault();
            //供应商
            var gys = GonYinShang.GetAll();
            gys.Insert(0, new Supplier() { Id = 99999999, SupplierName = "请选择供应商" });
            ViewBag.SupplierId = new SelectList(gys, "Id", "SupplierName",s.SupplierId);
            //单据类型
            var lty = inStorageType.GetAll();
            lty.Insert(0, new InStorageType() { Id = 9999, InSTypeName = "请选择入库单类型" });
            ViewBag.InSTypeId = new SelectList(lty, "Id", "InSTypeName",s.InSTypeId);
            //单据类型
            var product = productManager.GetAll();
            product.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product, "Id", "ProductName");
            //库位类型
            var location = locationManager.GetAll();
            location.Insert(0, new Location() { Id = 9999, LocationName = "请选择库位" });
            ViewBag.location = new SelectList(location, "Id", "LocationName");
            return View(s);
        }

        //根据id获取详细
        public ActionResult QueryByIdMinXiInfo(int id)
        {
            InStorage ins = inStorage.GetByWhere(i => i.Id == id).SingleOrDefault();
            var mx = inStorageDetail.GetByWhere(i => i.InStorageId==ins.InSNum && i.IsDelete == 0 && i.IsReturnOrder == 0);
            return Json(mx, JsonRequestBehavior.AllowGet);
        }

        //修改入库单
        public ActionResult UpdtInfo(List<InStorageDetail> detail, int inSTypeId, int supplierId, string Remark,string InSNum)
        {
            //先删除明细
            bool val_1 = true;
            var inStorageDetails = new InStorageDetailManager();
            var mx = inStorageDetails.GetByWhere(i => i.InStorageId == InSNum);
            foreach (var item in mx)
            {
                val_1 = inStorageDetails.Delete(item);
            }

            //获取明细表最大编号
            string detailNumBig = inStorageDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
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
                item.InStorageId = InSNum;
                item.CreateTime = DateTime.Now;
                val = inStorageDetail.Add(item);
            }
            if (val)
            {
                var num = inStorageDetail.GetByWhere(item => item.InStorageId == InSNum).Sum(item => item.Quantity);
                var sumMoney = Convert.ToDouble(inStorageDetail.GetByWhere(item => item.InStorageId == InSNum).Sum(item => item.SumMoney));
                var inStorage_1 = new InStorageManager();
                var s = inStorage_1.GetByWhere(i => i.InSNum== InSNum).SingleOrDefault();
                s.DetailNum = detailNum;
                s.InSTypeId = inSTypeId;
                s.SupplierId = supplierId;
                s.Remark = Remark;
                s.Num = Convert.ToInt32(num);
                s.SumMoney = Convert.ToDouble(sumMoney);
                bool vall = inStorage.Update(s);
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

        //删除入库单
        public ActionResult DeleteInfo(int id)
        {
            InStorage ins = inStorage.GetByWhere(item => item.Id == id).SingleOrDefault();
            List<InStorageDetail> listDetail = inStorageDetail.GetByWhere(item => item.InStorageId == ins.InSNum);
            bool val = true;
            string msg = "";
            foreach (var list in listDetail)
            {
                list.IsDelete = 1;
                val = inStorageDetail.Update(list);
            }
            if (val)
            {
                ins.IsDelete = 1;
                bool vall = inStorage.Update(ins);
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
        public ActionResult DeleteOther(List<InStorage> list)
        {
            string msg = "";
            foreach (var item in list)
            {
                InStorage ins = inStorage.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                List<InStorageDetail> listDetail = inStorageDetail.GetByWhere(i => i.InStorageId == ins.InSNum);
                bool val = true;
                foreach (var listd in listDetail)
                {
                    listd.IsDelete = 1;
                    val = inStorageDetail.Update(listd);
                }
                if (val)
                {
                    ins.IsDelete = 1;
                    bool vall = inStorage.Update(ins);
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