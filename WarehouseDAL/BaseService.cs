using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Linq.Expressions;
using System.Data.Entity;

namespace WarehouseDAL
{
    /// <summary>
    /// 数据访问基类
    /// </summary>
    public class BaseService<T> where T : class
    {
        DbContext dbContext = null;
        public DbContext MyDbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = new DbContext("name=WarehouseEntities");
                }
                return dbContext;
            }
        }

        /// <summary>
        /// 获取所有商品信息
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            return MyDbContext.Set<T>().ToList();
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetByWhere(Expression<Func<T, bool>> where)
        {
            return MyDbContext.Set<T>().Where(where).ToList();
        }

        public List<Function> GetByLinqRoleId(int roleId)
        {
            WarehouseEntities entities = new WarehouseEntities();
            var obj = from r in entities.RolePower
                      join f in entities.Function
                      on r.NodeId equals f.NodeId
                      where r.RoleId==roleId
                      select f;
            return obj.ToList();
        }

        public List<Function> GetByLinqRoleIdNot(int roleId)
        {
            WarehouseEntities entities = new WarehouseEntities();
            var obj = from f in entities.Function
                      where !
                      (from r in entities.RolePower join ff in entities.Function
                        on r.NodeId equals ff.NodeId
                              where r.RoleId == roleId
                              select ff.NodeId).Contains(f.NodeId)
                      select f;
            return obj.ToList();
        }

        public List<Function> GetByLinqParentNodeId(int roleId)
        {
            WarehouseEntities entities = new WarehouseEntities();

            //var test = (from r in entities.RolePower
            //            join fff in entities.Function
            //             on r.NodeId equals fff.NodeId
            //            where r.RoleId == roleId && fff.ParentNodeId!=0
            //            select fff.NodeId);
            //string t = test.ToString();
            var obj = from f in entities.Function
                      where (from ff in entities.Function
                              where !
                              (from r in entities.RolePower
                               join fff in entities.Function
                                on r.NodeId equals fff.NodeId
                               where r.RoleId == roleId && fff.ParentNodeId != 0
                               select fff.NodeId).Contains(ff.NodeId)
                              select ff.ParentNodeId).Contains(f.ParentNodeId)
                               && f.ParentNodeId == 0
                      select f;
            string s = obj.ToString();
            return obj.ToList();
        }

        /// <summary>
        /// 条件升序查询 带分页 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetByWhereAsc<orderByT>(Expression<Func<T, bool>> where, Expression<Func<T, orderByT>> orderBy, ref int pageIndex, ref int count, ref int pageCount, int pageSize)
        {
            count = MyDbContext.Set<T>().Where(where).Count(); //总条数
            pageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1; //总页数
            if (pageIndex <= 1 || count == 0) pageIndex = 1;
            else if (pageIndex >= pageCount) pageIndex = pageCount;

            var filterCount = (pageIndex - 1) * pageSize;
            return MyDbContext.Set<T>().Where(where).OrderBy(orderBy).Skip(filterCount).Take(pageSize).ToList();
        }

        /// <summary>
        /// 条件降序查询 带分页 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetByWhereDesc<orderByT>(Expression<Func<T, bool>> where, Expression<Func<T, orderByT>> orderBy, ref int pageIndex, ref int count, ref int pageCount, int pageSize)
        {
            count = MyDbContext.Set<T>().Where(where).Count(); //总条数
            pageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1; //总页数
            if (pageIndex <= 1 || count == 0) pageIndex = 1;
            else if (pageIndex >= pageCount) pageIndex = pageCount;

            var filterCount = (pageIndex - 1) * pageSize;
            return MyDbContext.Set<T>().Where(where).OrderByDescending(orderBy).Skip(filterCount).Take(pageSize).ToList();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(T model)
        {
            MyDbContext.Entry<T>(model).State = EntityState.Added;
            var result = MyDbContext.SaveChanges();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// 修改的实体 必须主键有值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(T model)
        {
            MyDbContext.Entry<T>(model).State = EntityState.Modified;
            var result = MyDbContext.SaveChanges();
            return result > 0;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(T model)
        {
            MyDbContext.Entry<T>(model).State = EntityState.Deleted;
            var result = MyDbContext.SaveChanges();
            return result > 0;
        }
    }
}
