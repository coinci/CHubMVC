//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CHubDBEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class TS_ORDER_HEADER
    {
        public decimal REQ_NO { get; set; }
        public string TO_SYSTEM { get; set; }
        public string CUSTOMER_NO { get; set; }
        public string NAME { get; set; }
        public decimal BILL_TO_LOCATION { get; set; }
        public decimal SHIP_TO_LOCATION { get; set; }
        public System.DateTime DUE_DATE { get; set; }
        public string ORDER_TYPE { get; set; }
        public string CUSTOMER_PO_NO { get; set; }
        public string SPL_IND { get; set; }
        public Nullable<decimal> DEST_LOCATION { get; set; }
        public string DEST_NAME { get; set; }
        public string DEST_ADDR_1 { get; set; }
        public string DEST_ADDR_2 { get; set; }
        public string DEST_CITY { get; set; }
        public string DEST_ZIP { get; set; }
        public string DEST_STATE { get; set; }
        public string DEST_COUNTRY { get; set; }
        public string SHIPCOMP_FLAG { get; set; }
        public string WAREHOUSE { get; set; }
        public string LOCAL_DEST_NAME { get; set; }
        public string LOCAL_DEST_ADDR_1 { get; set; }
        public string LOCAL_DEST_ADDR_2 { get; set; }
        public string LOCAL_DEST_ADDR_3 { get; set; }
        public string LOCAL_DEST_CITY { get; set; }
        public string LOCAL_DEST_COUNTRY { get; set; }
        public string LOCAL_DEST_STATE { get; set; }
        public System.DateTime CREATION_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> RECORD_DATE { get; set; }
        public string UPDATED_BY { get; set; }
    
        public virtual APP_ORDER_TYPE APP_ORDER_TYPE { get; set; }
        public virtual M_SYSTEM M_SYSTEM { get; set; }
    }
}
