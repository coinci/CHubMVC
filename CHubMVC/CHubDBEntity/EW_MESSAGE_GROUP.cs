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
    
    public partial class EW_MESSAGE_GROUP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EW_MESSAGE_GROUP()
        {
            this.EW_MESSAGE = new HashSet<EW_MESSAGE>();
        }
    
        public string EW_GROUP { get; set; }
        public string EW_GROUP_DESC { get; set; }
        public string ACTIVEIND { get; set; }
        public string GROUP_DESC_SHORT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EW_MESSAGE> EW_MESSAGE { get; set; }
    }
}
