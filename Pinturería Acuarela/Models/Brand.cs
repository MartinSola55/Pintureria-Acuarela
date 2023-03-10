namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Brand")]
    public partial class Brand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Brand()
        {
            Product = new HashSet<Product>();
        }

        public int id { get; set; }

        [Required(ErrorMessage = "Debes ingresar un nombre")]
        [StringLength(255, ErrorMessage = "Debes ingresar un nombre de menos de 255 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s'0-9.]+$", ErrorMessage = "Ingrese un nombre válido")]
        public string name { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }
    }
}
