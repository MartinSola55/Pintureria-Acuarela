namespace Pintureria_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Producto")]
    public partial class Producto
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "Hola que tal")]
        [RegularExpression("[0-9]", ErrorMessage = "Debe ser un numero")]
        public int cantidad { get; set; }

        public int sucursal { get; set; }
    }
}
