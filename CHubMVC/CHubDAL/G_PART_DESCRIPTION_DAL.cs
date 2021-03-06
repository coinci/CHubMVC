﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHubDBEntity;
using static CHubCommon.CHubEnum;

namespace CHubDAL
{
    public class G_PART_DESCRIPTION_DAL : BaseDAL
    {
        public G_PART_DESCRIPTION_DAL()
            : base() { }

        public G_PART_DESCRIPTION_DAL(CHubEntities db)
            : base(db) { }

        public bool IsPartNoExist(string partNo)
        {
            return db.G_PART_DESCRIPTION.Any(a => a.PART_NO == partNo);
        }

        public G_PART_DESCRIPTION GetPartDescription(string partNo)
        {
            return db.G_PART_DESCRIPTION.FirstOrDefault(a => a.PART_NO == partNo);
        }

        public List<G_PART_DESCRIPTION> fuzzyqueryByPartNo(string fuzzyPartNo)
        {
            return db.G_PART_DESCRIPTION.Where(a => a.PART_NO.Contains(fuzzyPartNo)).ToList();
        }

        public bool IsInActive(string partNo)
        {
            string status = PartStatusEnum.I.ToString();
            return db.G_PART_DESCRIPTION.Any(a => a.PART_NO == partNo && a.PART_STATUS == status);
        }

    }
}
