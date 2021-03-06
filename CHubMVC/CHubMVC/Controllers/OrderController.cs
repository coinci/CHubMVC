﻿using CHubCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CHubDBEntity;
using CHubBLL;
using CHubModel.ExtensionModel;
using CHubModel;
using System.Net;
using System.Text;
using static CHubCommon.CHubEnum;
using CHubMVC.Validations;

namespace CHubMVC.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Order
        [Authorize]
        public ActionResult Index(string seq)
        {
            //if (Session[CHubConstValues.SessionUser] == null)
            //    //Session[CHubConstValues.SessionUser] = "lg166";// For test using
            //   return RedirectToAction("Login", "Account");

            ViewBag.AppUser = Session[CHubConstValues.SessionUser].ToString();
            ViewBag.seq = seq;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Init()
        {
            //if (Session[CHubConstValues.SessionUser] == null)
            //    return RedirectToAction("Login", "Account");

            using (CHubEntities db = new CHubEntities())
            {
                string appUser = Session[CHubConstValues.SessionUser].ToString();
                //add recent page data
                APP_RECENT_PAGES_BLL rpBLL = new APP_RECENT_PAGES_BLL(db);
                rpBLL.Add(appUser, PageNameEnum.qkord.ToString(), this.Request.Url.AbsoluteUri);

                APP_CUST_ALIAS_BLL acaBLL = new APP_CUST_ALIAS_BLL(db);
                List<ExAppCustAlias> acaList = acaBLL.GetAppCustAliasByAppUser(appUser);

                APP_ORDER_TYPE_BLL aotBLL = new APP_ORDER_TYPE_BLL(db);
                List<APP_ORDER_TYPE> aotList = aotBLL.GetValidOrderType();
                foreach (var item in aotList)
                {
                    item.TS_OR_HEADER = null;
                }

                var obj = new
                {
                    custAlias = acaList,
                    orderType = aotList,
                    defaultOrderType = CHubConstValues.DefaultOrderType
                };

                return Json(obj);
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult InitOrder(decimal orderSeq)
        {
            //if (Session[CHubConstValues.SessionUser] == null)
            //    return RedirectToAction("Login", "Account");

            using (CHubEntities db = new CHubEntities())
            {
                string appUser = Session[CHubConstValues.SessionUser].ToString();

                ExVAliasAddr priAddr = null;
                ExVAliasAddr AltAddr = null;
                List<OrderLineItem> olReslut = new List<OrderLineItem>();
                bool isSaved = false;
                bool splInd = true;
                string poNum = "";
                string dueDate = "";
                string note = "";

                V_ALIAS_ADDR_DFLT_BLL dfltBLL = new V_ALIAS_ADDR_DFLT_BLL(db);
                V_ALIAS_ADDR_SPL_BLL splBLL = new V_ALIAS_ADDR_SPL_BLL(db);

                TS_OR_HEADER_BLL hBLL = new TS_OR_HEADER_BLL(db);
                List<TS_OR_HEADER> hList = hBLL.GetHeadersBySeq(orderSeq);
                if (hList != null && hList.Count > 0)
                {
                    isSaved = true;
                    //Get primary addr and Alt addr
                    foreach (var item in hList)
                    {
                        //special ship
                        if (item.SPL_IND == CHubConstValues.IndY)
                        {
                            splInd = true;
                            if (item.SHIPFROM_SEQ == 0)
                            {
                                poNum = item.CUSTOMER_PO_NO;
                                dueDate = item.DUE_DATE.ToString("yyyy-MM-dd");
                                note = item.ORDER_NOTES;
                                priAddr = splBLL.GetSpecifyAliasAddrSPL(item.ALIAS_NAME, item.TO_SYSTEM, item.CUSTOMER_NO, item.BILL_TO_LOCATION, item.SHIP_TO_LOCATION, item.DEST_LOCATION);
                            }
                            if (item.SHIPFROM_SEQ == 1)
                                AltAddr = splBLL.GetSpecifyAliasAddrSPL(item.ALIAS_NAME, item.TO_SYSTEM, item.CUSTOMER_NO, item.BILL_TO_LOCATION, item.SHIP_TO_LOCATION, item.DEST_LOCATION);
                        }
                        else
                        {
                            splInd = false;
                            if (item.SHIPFROM_SEQ == 0)
                            {
                                poNum = item.CUSTOMER_PO_NO;
                                dueDate = item.DUE_DATE.ToString("yyyy-MM-dd");
                                note = item.ORDER_NOTES;
                                priAddr = dfltBLL.GetSpecifyAliasAddrDFLT(item.ALIAS_NAME, item.TO_SYSTEM, item.CUSTOMER_NO, item.BILL_TO_LOCATION, item.SHIP_TO_LOCATION);
                            }
                            if (item.SHIPFROM_SEQ == 1)
                                AltAddr = dfltBLL.GetSpecifyAliasAddrDFLT(item.ALIAS_NAME, item.TO_SYSTEM, item.CUSTOMER_NO, item.BILL_TO_LOCATION, item.SHIP_TO_LOCATION);
                        }
                    }

                    TS_OR_DETAIL_BLL dBLL = new TS_OR_DETAIL_BLL(db);
                    List<TS_OR_DETAIL> dList = dBLL.GetDetailsBySeq(orderSeq);

                    //change detail ot orderLine
                    OrderLineCheckArg arg = new OrderLineCheckArg();
                    arg.primarySysID = priAddr.SysID;
                    arg.primaryWareHouse = priAddr.WareHouse;
                    arg.customerNo = priAddr.CustomerNo;
                    if (AltAddr != null)
                    {
                        arg.altSysID = AltAddr.SysID;
                        arg.altWareHosue = AltAddr.WareHouse;
                    }
                    foreach (var item in dList)
                    {
                        arg.olItem = new OrderLineItem();
                        arg.olItem.CustomerPartNo = item.CUSTOMER_PART_NO;
                        arg.olItem.Qty = item.BUY_QTY;
                        arg.olItem.OrderLineNo = item.ORDER_LINE_NO;
                        CheckOrderLineItemAction(arg);
                        olReslut.Add(arg.olItem);
                    }
                }
                else
                {
                    TS_OR_HEADER_STAGE_BLL hsBLL = new TS_OR_HEADER_STAGE_BLL(db);
                    List<TS_OR_HEADER_STAGE> hsList = hsBLL.GetHeaderStageBySeq(orderSeq);

                    if (hsList != null && hsList.Count > 0)
                    {
                        isSaved = false;
                        //Get primary addr and Alt addr stage
                        foreach (var item in hsList)
                        {
                            //special ship
                            if (item.SPL_IND == CHubConstValues.IndY)
                            {
                                splInd = true;
                                if (item.SHIPFROM_SEQ == 0)
                                {
                                    poNum = item.CUSTOMER_PO_NO;
                                    dueDate = item.DUE_DATE.ToString("yyyy-MM-dd");
                                    note = item.ORDER_NOTES;
                                    priAddr = splBLL.GetSpecifyAliasAddrSPL(item.ALIAS_NAME, item.TO_SYSTEM, item.CUSTOMER_NO, item.BILL_TO_LOCATION, item.SHIP_TO_LOCATION, item.DEST_LOCATION);
                                }
                            }
                            else
                            {
                                splInd = false;
                                if (item.SHIPFROM_SEQ == 0)
                                {
                                    poNum = item.CUSTOMER_PO_NO;
                                    dueDate = item.DUE_DATE.ToString("yyyy-MM-dd");
                                    note = item.ORDER_NOTES;
                                    priAddr = dfltBLL.GetSpecifyAliasAddrDFLT(item.ALIAS_NAME, item.TO_SYSTEM, item.CUSTOMER_NO, item.BILL_TO_LOCATION, item.SHIP_TO_LOCATION);
                                }
                            }
                        }


                        TS_OR_DETAIL_STAGE_BLL dsBLL = new TS_OR_DETAIL_STAGE_BLL(db);
                        List<TS_OR_DETAIL_STAGE> dsList = dsBLL.GetDetailsStageByOrderSeq(orderSeq);

                        //change detail ot orderLine
                        OrderLineCheckArg arg = new OrderLineCheckArg();
                        arg.primarySysID = priAddr.SysID;
                        arg.primaryWareHouse = priAddr.WareHouse;
                        arg.customerNo = priAddr.CustomerNo;
                        arg.altSysID = null;
                        arg.altWareHosue = null;

                        foreach (var item in dsList)
                        {
                            arg.olItem = new OrderLineItem();
                            arg.olItem.CustomerPartNo = item.CUSTOMER_PART_NO;
                            arg.olItem.Qty = item.BUY_QTY;
                            arg.olItem.OrderLineNo = item.ORDER_LINE_NO;
                            CheckOrderLineItemAction(arg);
                            olReslut.Add(arg.olItem);
                        }
                    }
                    else
                    {
                        this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Content("Wrong Order Seq");
                    }
                }

                var obj = new
                {
                    priAddr = priAddr,
                    altAddr = AltAddr,
                    orderLines = olReslut,
                    isSaved = isSaved,
                    splInd = splInd,
                    poNum = poNum,
                    dueDate = dueDate,
                    note = note
                };

                return Json(obj);
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchAddrs(string shipName, string addr,string aliasName, bool isSpecialShip,string destName, long? destLocation)
        {
            try
            {
                List<ExVAliasAddr> list = new List<ExVAliasAddr>();
                CHubEntities db = new CHubEntities();
                if (isSpecialShip)
                {
                    V_ALIAS_ADDR_SPL_BLL bll = new V_ALIAS_ADDR_SPL_BLL();
                    list = bll.GetAliasAddrSPL(shipName.Trim(), addr.Trim(),destName.Trim(),destLocation, aliasName);
                }
                else
                {
                    V_ALIAS_ADDR_DFLT_BLL bll = new V_ALIAS_ADDR_DFLT_BLL();
                    list = bll.GetAliasAddrDFLT(aliasName);
                }

                //Get from parameter table***
                if (list.Count > 20)
                {
                    return Json(new RequestResult(false, string.Format("Result has {0} items, Make Condition more strict", list.Count.ToString())));
                }
                if (list.Count == 0)
                {
                    return Json(new RequestResult(false, "No result"));
                }

                return Json(new RequestResult(list));
            }
            catch (Exception ee)
            {
                return Json(new RequestResult(false, ee.Message));
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetStrictAddrs(string shipName, string addr, string aliasName, bool isSpecialShip)
        {
            try
            {
                List<ExVAliasAddr> list = new List<ExVAliasAddr>();
                CHubEntities db = new CHubEntities();
                if (isSpecialShip)
                {
                    V_ALIAS_ADDR_SPL_BLL bll = new V_ALIAS_ADDR_SPL_BLL();
                    list = bll.GetStrictAliasAddrSPL(shipName.Trim(), addr.Trim(), aliasName);
                }
                else
                {
                    V_ALIAS_ADDR_DFLT_BLL bll = new V_ALIAS_ADDR_DFLT_BLL();
                    list = bll.GetStrictAliasAddrDFLT(shipName.Trim(), addr.Trim(), aliasName);
                }
                return Json(list);
            }
            catch (Exception ee)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ee.Message);
            }
        }


        [HttpPost]
        [Authorize]
        public ActionResult SaveDraft(OrderSaveArg arg)
        {
            try
            {
                if (arg.headInfo == null)
                    return Content("fail");

                string appUser = Session[CHubConstValues.SessionUser].ToString();

                CHubEntities db = new CHubEntities();
                TS_OR_HEADER_STAGE_BLL bll = new TS_OR_HEADER_STAGE_BLL(db);

                //Header part
                TS_OR_HEADER_STAGE orHeaderStage = ManualClassConvert.ConvertExAliaAddr2HeaderStage(arg.headInfo,arg.seq, arg.dueDate,arg.orderType,arg.shipCompFlag,arg.customerPONO, arg.orderNote,arg.isSpecialShip,appUser);
                TS_OR_HEADER_STAGE altORHeaderStage = null;
                if (arg.altHeadInfo != null)
                {
                    altORHeaderStage = ManualClassConvert.ConvertExAliaAddr2HeaderStage(arg.altHeadInfo,arg.seq, arg.dueDate, arg.orderType, arg.shipCompFlag,arg.customerPONO, arg.orderNote, arg.isSpecialShip,appUser,true);
                }

                //Detail Part
                List<TS_OR_DETAIL_STAGE> dStageList = null;
                if (arg.olList != null && arg.olList.Count > 0)
                {
                    dStageList = new List<TS_OR_DETAIL_STAGE>();
                    foreach (var item in arg.olList)
                    {
                        //ignore wrong lines
                        if (string.IsNullOrEmpty(item.PartNo) || string.IsNullOrEmpty(item.PriAVLCheckColor))
                            continue;
                        TS_OR_DETAIL_STAGE dStage = ManualClassConvert.ConvertOLItem2DetailStage(item, arg.seq, appUser);
                        dStageList.Add(dStage);
                    }
                }

                decimal seq = 0;
                if (string.IsNullOrEmpty(arg.seq))
                    seq = bll.AddHeadersWithDetailsStage(orHeaderStage, altORHeaderStage,dStageList);
                else
                    seq = bll.UpdateHeadersWithDetailsStage(orHeaderStage, altORHeaderStage,dStageList);

                if (seq != 0.00M)
                    return Content(seq.ToString());
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Fail to save draft");
                }
            }
            catch(Exception ee)
            {
                //log ee
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ee.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveOrder(OrderSaveArg arg)
        {
            try
            {
                if (arg.headInfo == null)
                    return Content("fail");

                string appUser = Session[CHubConstValues.SessionUser].ToString();

                CHubEntities db = new CHubEntities();
                TS_OR_HEADER_BLL bll = new TS_OR_HEADER_BLL(db);

                //Header part
                TS_OR_HEADER orHeader = ManualClassConvert.ConvertExAliaAddr2Header(arg.headInfo, arg.seq, arg.dueDate, arg.orderType, arg.shipCompFlag, arg.customerPONO, arg.orderNote,arg.isSpecialShip, appUser);
                TS_OR_HEADER altORHeader = null;
                if (arg.altHeadInfo != null)
                {
                    altORHeader = ManualClassConvert.ConvertExAliaAddr2Header(arg.altHeadInfo,arg.seq, arg.dueDate, arg.orderType, arg.shipCompFlag, arg.customerPONO, arg.orderNote, arg.isSpecialShip,appUser, true);
                }

                //Detail part
                List<TS_OR_DETAIL> detailList = null;
                if (arg.olList != null && arg.olList.Count > 0)
                {
                    detailList = new List<TS_OR_DETAIL>();
                    foreach (var item in arg.olList)
                    {
                        //ignore wrong lines
                        if (string.IsNullOrEmpty(item.PartNo) || string.IsNullOrEmpty(item.PriAVLCheckColor))
                            continue;
                        TS_OR_DETAIL detail = ManualClassConvert.ConvertOLItem2Detail(item, arg.seq, appUser);
                        detailList.Add(detail);
                    }
                }

                decimal seq = 0;
                if (string.IsNullOrEmpty(arg.seq))
                    seq = bll.AddHeadersWithDetails(orHeader, altORHeader,detailList);
                else
                    seq = bll.UpdateHeadersWithDetails(orHeader, altORHeader,detailList);

                if (seq != 0.00M)
                    return Content(seq.ToString());
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Fail to save order");
                }
            }
            catch (Exception ee)
            {
                //log ee
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ee.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult CheckOrderLineItem(OrderLineCheckArg olArg)
        {
            try
            {
                string msg = CheckOrderLineItemAction(olArg);
                if (string.IsNullOrEmpty(msg))
                    return Json(olArg.olItem);
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Content(msg);
                }
            }
            catch (Exception ee)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ee.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult BatchCheckOrderLines(OrderLineBatchCheckArg arg)
        {
            try
            {
                List<OrderLineItem> olList = new List<OrderLineItem>();

                OrderLineCheckArg olArg = new OrderLineCheckArg
                {
                    primarySysID = arg.primarySysID,
                    primaryWareHouse = arg.primaryWareHouse,
                    altSysID = arg.altSysID,
                    altWareHosue = arg.altWareHosue,
                    customerNo = arg.customerNo
                };
                foreach (var item in arg.olItemList)
                {
                    olArg.olItem = item;
                    CheckOrderLineItemAction(olArg);
                    olList.Add(olArg.olItem);
                }
                return Json(olList);
            }
            catch (Exception ex)
            {
                //log
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content(ex.Message);
            }
        }

        [Authorize]
        public ActionResult DownLoadOrder(decimal? orderSeq,decimal shipFrom = 0)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                if (orderSeq == null || orderSeq == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Content("No order seq data");
                }

                CHubEntities db = new CHubEntities();
                V_O_DOWNLOAD_HDR_BLL hBLL = new V_O_DOWNLOAD_HDR_BLL(db);
                V_O_DOWNLOAD_HDR hData = hBLL.GetSpecfyHDRData(orderSeq.Value, shipFrom);
                if (hData == null)
                    throw new Exception("No Header Data");

                V_O_DOWNLOAD_DTL_BLL dBLL = new V_O_DOWNLOAD_DTL_BLL(db);
                List<V_O_DOWNLOAD_DTL> dList = dBLL.GetDTLList(orderSeq.Value, shipFrom);

                if (dList == null || dList.Count == 0)
                    throw new Exception("No Lines Data");


                sb.AppendLine(hData.TXT);
                foreach (var item in dList)
                {
                    sb.AppendLine(item.TXT);
                }

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

                return File(buffer, "text/csv", hData.FILE_NAME);
            }
            catch (Exception ee)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content(ee.Message);
            }
        }

        #region For draft function part
        [Authorize]
        public ActionResult Draft()
        {
            //if (Session[CHubConstValues.SessionUser] == null)
            //    return RedirectToAction("Login", "Account");

            string appUser = Session[CHubConstValues.SessionUser].ToString();
            APP_RECENT_PAGES_BLL rpBLL = new APP_RECENT_PAGES_BLL();
            rpBLL.Add(appUser, PageNameEnum.orddft.ToString(), this.Request.Url.AbsoluteUri);

            return View();
        }

        [Authorize]
        public ActionResult InitDraft()
        {
            //if (Session[CHubConstValues.SessionUser] == null)
            //    return RedirectToAction("Login", "Account");

            string appUser = Session[CHubConstValues.SessionUser].ToString();

            CHubEntities db = new CHubEntities();
            TS_OR_HEADER_STAGE_BLL hStageBLL = new TS_OR_HEADER_STAGE_BLL();
            List<TS_OR_HEADER_STAGE> hStageList = hStageBLL.GetHeaderStageByUser(appUser);

            return Json(hStageList);
        }


        [HttpPost]
        [Authorize]
        public ActionResult DeleteDraft(decimal orderSeq, decimal shipFrom)
        {
            try
            {
                CHubEntities db = new CHubEntities();
                TS_OR_HEADER_STAGE_BLL hStageBLL = new TS_OR_HEADER_STAGE_BLL();
                hStageBLL.DeleteDraft(orderSeq, shipFrom);

                return Content("Success");
            }
            catch (Exception ee)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ee.Message);
            }
        }

        #endregion

        #region  For Order List View

        [Authorize]
        public ActionResult Query()
        {
            //if (Session[CHubConstValues.SessionUser] == null)
            //    return RedirectToAction("Login", "Account");

            //add recent history
            string appUser = Session[CHubConstValues.SessionUser].ToString();
            APP_RECENT_PAGES_BLL rpBLL = new APP_RECENT_PAGES_BLL();
            rpBLL.Add(appUser, PageNameEnum.ordinq.ToString(), this.Request.Url.AbsoluteUri);

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult QueryAction(decimal? orderSeq,string custAlias,string poNum, int currentPage, int pageSize)
        {
            if (Session[CHubConstValues.SessionUser] == null)
                return RedirectToAction("Login", "Account");

            int totalCount = 0;
            CHubEntities db = new CHubEntities();
            TS_OR_HEADER_BLL hBLL = new TS_OR_HEADER_BLL(db);
            List<TS_OR_HEADER> result = hBLL.GetHeaders(orderSeq, custAlias, poNum,  currentPage,  pageSize,out totalCount);

            var obj = new
            {
                result = result,
                totalCount = totalCount
            };
            return Json(obj);
        }

        #endregion





        #region *** private function ***
        private string GetPartNoFromCustPartNo(string custPartNo,string customerNo)
        {
            if (string.IsNullOrEmpty(custPartNo))
                return string.Empty;
            using (CHubEntities db = new CHubEntities())
            {
                G_CATALOG_CUSTOMER_PART_BLL custBLL = new G_CATALOG_CUSTOMER_PART_BLL(db);
                string PartNo = custBLL.GetPartNoFromCustPartNo(custPartNo,customerNo);
                if (string.IsNullOrEmpty(PartNo))
                {
                    G_PART_DESCRIPTION_BLL partBLL = new G_PART_DESCRIPTION_BLL(db);
                    if (partBLL.IsPartNoExist(custPartNo))
                        return custPartNo;
                    else
                    {
                        return string.Empty;
                    }

                }
                return PartNo;
            }
        }

        public string CheckOrderLineItemAction(OrderLineCheckArg olArg)
        {
            //reset 
            olArg.olItem.PartNo = string.Empty;
            olArg.olItem.AltAVLCheckColor = string.Empty;
            olArg.olItem.PriAVLCheckColor = string.Empty;
            olArg.olItem.PartNoPlaceHolder = string.Empty;
            olArg.olItem.PriAVLCheck = null;
            olArg.olItem.AltAVLCheck = null;
            olArg.olItem.DescCN = string.Empty;
            olArg.olItem.Description = string.Empty;
            olArg.olItem.ItemBackColor = string.Empty;
            olArg.olItem.WarningMsg = string.Empty;
            olArg.olItem.WarningColor = string.Empty;

            olArg.olItem.LastCheckNo = olArg.olItem.CustomerPartNo;
            olArg.olItem.LastQty = olArg.olItem.Qty;

            string msg = string.Empty;

            olArg.olItem.PartNo = GetPartNoFromCustPartNo(olArg.olItem.CustomerPartNo,olArg.customerNo);


            olArg.olItem.PartNoPlaceHolder = string.Empty;
            if (string.IsNullOrEmpty(olArg.olItem.PartNo))
            {
                olArg.olItem.PartNoPlaceHolder = "Can't find Part NO";
                olArg.olItem.ItemBackColor = CHubConstValues.ErrorColor;
                return null;
            }
            else
            {
                CHubEntities db = new CHubEntities();
                G_PART_DESCRIPTION_BLL pDescBLL = new G_PART_DESCRIPTION_BLL(db);

                //Get description
                G_PART_DESCRIPTION pDesc = pDescBLL.GetPartDescription(olArg.olItem.PartNo);
                olArg.olItem.Description = pDesc.DESCRIPTION;
                olArg.olItem.DescCN = pDesc.DESC_CN;

                //check inactive status
                if (pDesc.PART_STATUS == PartStatusEnum.I.ToString())
                {
                    olArg.olItem.WarningMsg = string.Format("SSC:{0},", pDesc.CURRENT_SALES_STATUS_CODE);
                    //olArg.olItem.WarningColor = CHubConstValues.WarningColor;
                    
                }

                string usingSysID = olArg.primarySysID;
                //Do AVL check
                if (olArg.olItem.Qty > 0)
                {
                    if (string.IsNullOrEmpty(olArg.primarySysID) || string.IsNullOrEmpty(olArg.primaryWareHouse))
                    {
                        msg = "No Primary SysID and WareHouse information";
                    }


                    G_NETAVL_BLL netBLL = new G_NETAVL_BLL(db);
                    //Primary AVL check
                    decimal priNet = netBLL.GetSpecifyNETAVL(olArg.primarySysID, olArg.olItem.PartNo, olArg.primaryWareHouse);
                    if (priNet == 0)
                        olArg.olItem.PriAVLCheckColor = CHubConstValues.NoStockColor;
                    else if (priNet >= olArg.olItem.Qty)
                        olArg.olItem.PriAVLCheckColor = CHubConstValues.SatisfyStockColor;
                    else
                        olArg.olItem.PriAVLCheckColor = CHubConstValues.PartialStockColor;
                    olArg.olItem.PriAVLCheck = priNet;

                    //if primary is no enough do  Alt AVL check
                    if (priNet < olArg.olItem.Qty)
                    {
                        if (!(string.IsNullOrEmpty(olArg.altSysID) || string.IsNullOrEmpty(olArg.altWareHosue)))
                        {
                            decimal altNet = netBLL.GetSpecifyNETAVL(olArg.altSysID, olArg.olItem.PartNo, olArg.altWareHosue);
                            if (altNet == 0)
                                olArg.olItem.AltAVLCheckColor = CHubConstValues.NoStockColor;
                            else if (altNet >= olArg.olItem.Qty)
                            {
                                olArg.olItem.AltAVLCheckColor = CHubConstValues.SatisfyStockColor;
                                usingSysID = olArg.altSysID;
                            }
                            else
                                olArg.olItem.AltAVLCheckColor = CHubConstValues.PartialStockColor;
                            olArg.olItem.AltAVLCheck = altNet;
                        }
                    }
                }

                //Do G_OESALES_CATALOG_VALIDATION
                G_OESALES_CATALOG_VALIDATION oeSaleValidation = new G_OESALES_CATALOG_VALIDATION(usingSysID, olArg.olItem.PartNo,olArg.olItem.Qty);
                olArg.olItem.WarningMsg += oeSaleValidation.ValidationAction();
                if(!string.IsNullOrEmpty(olArg.olItem.WarningMsg))
                    olArg.olItem.WarningColor = CHubConstValues.WarningColor;
            }

            return msg;
        }

        #endregion


    }
}
