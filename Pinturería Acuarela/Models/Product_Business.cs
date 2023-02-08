namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product_Business
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_product { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_business { get; set; }

        public int? minimum_stock { get; set; }

        public int stock { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        public virtual Business Business { get; set; }

        public virtual Product Product { get; set; }
    }
}
