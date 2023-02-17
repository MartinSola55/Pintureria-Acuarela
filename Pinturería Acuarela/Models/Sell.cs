namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sell")]
    public partial class Sell
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sell()
        {
            Product_Sell = new HashSet<Product_Sell>();
        }

        public int id { get; set; }

        public DateTime date { get; set; }

        public int id_user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Sell> Product_Sell { get; set; }

        public virtual User User { get; set; }
    }
}
