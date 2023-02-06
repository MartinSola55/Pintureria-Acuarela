namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product_Order
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_product { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id_order { get; set; }

        public int quantity { get; set; }

        public bool status { get; set; }

        public int quantity_send { get; set; }

        public int? id_business_sender { get; set; }

        public virtual Business Business { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
