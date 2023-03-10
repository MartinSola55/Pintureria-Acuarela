namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Order = new HashSet<Order>();
            Sell = new HashSet<Sell>();
        }

        public int id { get; set; }

        [Required(ErrorMessage = "Por favor, ingrese un email")]
        [StringLength(60, ErrorMessage = "Ingresa un email de menos de 60 caracteres")]
        [EmailAddress(ErrorMessage = "Por favor, ingrese un email válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "Por favor, ingrese una contraseña")]
        [StringLength(20, ErrorMessage = "Ingresa una contraseña de menos de 20 caracteres")]
        public string password { get; set; }

        public short id_rol { get; set; }

        public int? id_business { get; set; }

        public virtual Business Business { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }

        public virtual Rol Rol { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sell> Sell { get; set; }
    }
}
