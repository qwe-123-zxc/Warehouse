//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class InStorage
    {
        public int Id { get; set; }
        public string InSNum { get; set; }
        public int InSTypeId { get; set; }
        public int SupplierId { get; set; }
        public string DetailNum { get; set; }
        public int Num { get; set; }
        public double SumMoney { get; set; }
        public string Status { get; set; }
        public string AuditUser { get; set; }
        public System.DateTime AuditTime { get; set; }
        public int IsDelete { get; set; }
        public string Remark { get; set; }
    
        public virtual InStorageType InStorageType { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
