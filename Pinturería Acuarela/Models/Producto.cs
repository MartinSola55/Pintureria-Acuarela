namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Producto")]
    public partial class Producto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        public int quantity { get; set; }

        public double price { get; set; }

        public int id_sucursal { get; set; }

        [StringLength(255)]
        public string description { get; set; }

        public virtual Sucursal Sucursal { get; set; }
    }
}
