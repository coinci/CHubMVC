﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHubDBEntity;

namespace CHubDAL
{
    public class V_ITT_SHIPPING_ALLIN1_DAL : BaseDAL
    {
        public V_ITT_SHIPPING_ALLIN1_DAL()
            : base() { }

        public V_ITT_SHIPPING_ALLIN1_DAL(CHubEntities db)
            : base(db) { }

        public List<V_ITT_SHIPPING_ALLIN1> GetShipmentData(string partNo)
        {
            return db.V_ITT_SHIPPING_ALLIN1.Where(a => a.PART_NO == partNo).OrderByDescending(a=>a.SHIP_DATE).ToList();
        }

        public List<V_ITT_SHIPPING_ALLIN1> GetLevel4Data(string partNo,string orderNo,long lineNo)
        {
            return db.V_ITT_SHIPPING_ALLIN1.Where(a => a.PART_NO == partNo
            &&a.ORDER_NO==orderNo && a.LINE_NO.Value==lineNo
            ).OrderByDescending(a => a.SHIP_DATE).ToList();
        }

    }
}
