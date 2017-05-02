﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CHubBLL;
using CHubDBEntity;
using CHubModel;
using CHubCommon;
using static CHubCommon.CHubEnum;
using CHubModel.ExtensionModel;

namespace CHubMVC.Controllers
{
    public class KPIController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetKPIGroupList()
        {
            try
            {
                DB_KPI_GROUP_BLL gBLL = new DB_KPI_GROUP_BLL();
                List<DB_KPI_GROUP> result = gBLL.GetKPIGroups();

                return Json(new RequestResult(result));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetKPIGroupList", ex);
                return Json(new RequestResult(false, ex.Message));
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetLatestHistory(string kpiGroup)
        {
            try
            {
                DB_KPI_HISTORY_BLL hBLL = new DB_KPI_HISTORY_BLL();
                List<ExDBKPIHistory> result = hBLL.GetLatestHistory(kpiGroup);
                List<string> codeList = hBLL.GetDistinctKPICode(kpiGroup);

                var obj = new
                {
                    History = result,
                    Group = codeList
                };
                return Json(new RequestResult(obj));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetLatestHistory", ex);
                return Json(new RequestResult(false, ex.Message));
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetTrendData(string code,string subCode)
        {
            try
            {
                DB_KPI_HISTORY_BLL hBLL = new DB_KPI_HISTORY_BLL();
                List<DB_KPI_HISTORY> hList = hBLL.GetTrendData(code,subCode);
                if(hList == null || hList.Count==0)
                    return Json(new RequestResult(null));

                List<DateTime> kpiDates = new List<DateTime>();
                List<decimal> kpiValues = new List<decimal>();
                List<decimal> kpiTarget = new List<decimal>();
                foreach (var h in hList)
                {
                    kpiDates.Add(h.KPI_DATE);
                    kpiValues.Add(h.KPI_VALUE);
                    kpiTarget.Add(h.KPI_TARGET.Value);
                }

                var obj = new
                {
                    kpiDates = kpiDates,
                    kpiValues = kpiValues,
                    kpiTarget = kpiTarget
                };
                return Json(new RequestResult(obj));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetTrendData", ex);
                return Json(new RequestResult(false, ex.Message));
            }
        }


    }
}