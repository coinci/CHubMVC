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
    
    public partial class APP_USER_ROLE_LINK
    {
        public string APP_USER { get; set; }
        public string ROLE_NAME { get; set; }
        public string COMMENTS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
    
        public virtual APP_ROLES APP_ROLES { get; set; }
        public virtual APP_USERS APP_USERS { get; set; }
    }
}
