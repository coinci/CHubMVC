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
    
    public partial class APP_CUST_ALIAS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public APP_CUST_ALIAS()
        {
            this.APP_CUST_ALIAS_LINK = new HashSet<APP_CUST_ALIAS_LINK>();
            this.APP_USER_ALIAS_LINK = new HashSet<APP_USER_ALIAS_LINK>();
        }
    
        public string ALIAS_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string ACTIVEIND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APP_CUST_ALIAS_LINK> APP_CUST_ALIAS_LINK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APP_USER_ALIAS_LINK> APP_USER_ALIAS_LINK { get; set; }
    }
}
