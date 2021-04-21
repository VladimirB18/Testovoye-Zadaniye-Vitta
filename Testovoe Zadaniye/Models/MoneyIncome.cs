namespace Testovoe_Zadaniye
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MoneyIncome")]
    public partial class MoneyIncome
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MoneyIncome()
        {
            Payments = new HashSet<Payments>();
        }

        public long Id { get; set; }

        public DateTime Date { get; set; }

        public double Summa { get; set; }

        public double? Balance { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payments> Payments { get; set; }
    }
}
