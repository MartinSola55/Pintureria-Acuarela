namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Color")]
    public partial class Color
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Color()
        {
            Product = new HashSet<Product>();
        }

        public int id { get; set; }

        [Required(ErrorMessage = "Debes ingresar un nombre")]
        [StringLength(255, ErrorMessage = "Debes ingresar un nombre de menos de 255 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s.]+$", ErrorMessage = "Ingrese un nombre válido")]
        public string name { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un color")]
        [StringLength(7, ErrorMessage = "Debes seleccionar un color válido")]
        [RegularExpression(@"^#[A-Fa-f0-9]{6}$", ErrorMessage = "Ingrese un código válido")]
        public string rgb_hex_code { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }
    }
}
