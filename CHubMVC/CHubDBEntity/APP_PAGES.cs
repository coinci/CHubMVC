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
    
    public partial class APP_PAGES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public APP_PAGES()
        {
            this.APP_PAGE_ROLE_LINK = new HashSet<APP_PAGE_ROLE_LINK>();
            this.APP_RECENT_PAGES = new HashSet<APP_RECENT_PAGES>();
            this.APP_SPACE_PAGE_LINK = new HashSet<APP_SPACE_PAGE_LINK>();
        }
    
        public string PAGE_NAME { get; set; }
        public string DISPLAY { get; set; }
        public string DESCRIPTION { get; set; }
        public string ACTIVEIND { get; set; }
        public string VERION { get; set; }
        public string URL { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
        public string PUBLICIND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APP_PAGE_ROLE_LINK> APP_PAGE_ROLE_LINK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APP_RECENT_PAGES> APP_RECENT_PAGES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APP_SPACE_PAGE_LINK> APP_SPACE_PAGE_LINK { get; set; }
    }
}
