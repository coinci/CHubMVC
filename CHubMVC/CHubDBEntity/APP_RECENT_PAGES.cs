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
    
    public partial class APP_RECENT_PAGES
    {
        public string APP_USER { get; set; }
        public string PAGE_NAME { get; set; }
        public System.DateTime LAST_ACTIVITY { get; set; }
    
        public virtual APP_PAGES APP_PAGES { get; set; }
        public virtual APP_USERS APP_USERS { get; set; }
    }
}
