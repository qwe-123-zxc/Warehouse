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
    
    public partial class InStorageDetail
    {
        public int Id { get; set; }
        public string DetailNum { get; set; }
        public string InStorageId { get; set; }
        public string ProductNum { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<int> THQuantity { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> THQuantity { get; set; }
        public Nullable<double> SumMoney { get; set; }
        public string Location { get; set; }
        public Nullable<int> IsReturnOrder { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int IsDelete { get; set; }
    }
}
