using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseWeb.Models.Dto
{
    //入库管理条件查询
    [Serializable]
    public class QueryInStorageDto : QueryBaseDto
    {
        public string InSNum { get; set; }

        public string suoyou { get; set; }
        public string dsh { get; set; }
        public string shtg { get; set; }
        public string shsb { get; set; }

        DateTime? state = DateTime.Now.AddDays(-30);
        public DateTime? State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }
        DateTime? end = DateTime.Now.AddDays(+30);
        public DateTime? End
        {
            get
            {
                return end;
            }

            set
            {
                end = value;
            }
        }

        int inSTypeId = 9999;
        public int InSTypeId
        {
            get
            {
                return inSTypeId;
            }

            set
            {
                inSTypeId = value;
            }
        }

        int supplierId = 99999999;
        public int SupplierId
        {
            get
            {
                return supplierId;
            }

            set
            {
                supplierId = value;
            }
        }
    }
}