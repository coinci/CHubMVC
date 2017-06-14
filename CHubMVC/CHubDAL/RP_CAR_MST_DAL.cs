﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHubDBEntity;
using CHubModel.ExtensionModel;

namespace CHubDAL
{
    public class RP_CAR_MST_DAL : BaseDAL
    {
        public RP_CAR_MST_DAL()
            : base() { }

        public RP_CAR_MST_DAL(CHubEntities db)
            : base(db) { }

        public List<RP_CAR_MST> GetCARListByCode(string carCode)
        {
            return db.RP_CAR_MST.Where(a => a.CARCOD == carCode).ToList();
        }

        public void SaveCARWayBillID(RP_CAR_MST model)
        {
            RP_CAR_MST exist = db.RP_CAR_MST.FirstOrDefault(a => a.WH_ID == model.WH_ID && a.CARCOD == model.CARCOD);
            exist.WAYBILL_ID = model.WAYBILL_ID;
            Update(exist);
        }

        public List<DistinctCarCode> GetDistinctCarCode()
        {
            var result = (
                from c in db.RP_CAR_MST
                select new DistinctCarCode {
                    CarCode = c.CARCOD,
                    CarName = c.CARNAM
                }
                ).Distinct().ToList();

            //some data have same carCode but diff carName, so need to handle
            List<DistinctCarCode> realList = new List<DistinctCarCode>();
            foreach (var item in result)
            {
                DistinctCarCode exist = realList.FirstOrDefault(a => a.CarCode == item.CarCode);
                if (exist == null)
                    realList.Add(item);
                else
                {
                    //carName get the longer one
                    if ((item.CarName ?? string.Empty).Length > (exist.CarName ?? string.Empty).Length)
                        exist.CarName = item.CarName;
                }
            }
            return realList;
        
        }

    }
}
