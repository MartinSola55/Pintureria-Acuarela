//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Brand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Brand()
        {
            this.Product = new HashSet<Product>();
        }
    
        public int id { get; set; }

        [Required(ErrorMessage = "Debes agregar un nombre")]
        [StringLength(255, ErrorMessage = "Debes añadir un nombre de menos de 255 caracteres")]
        public string name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }
    }
}
