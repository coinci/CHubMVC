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
    
    public partial class EW_SCHEDULE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EW_SCHEDULE()
        {
            this.EW_SCHEDULE_TASK = new HashSet<EW_SCHEDULE_TASK>();
        }
    
        public string EW_SCHEDULE_ID { get; set; }
        public string EW_SCHEDULE_DESC { get; set; }
        public Nullable<decimal> WEEK { get; set; }
        public Nullable<decimal> HOUR { get; set; }
        public Nullable<decimal> MIN { get; set; }
        public string ACTIVEIND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EW_SCHEDULE_TASK> EW_SCHEDULE_TASK { get; set; }
    }
}
