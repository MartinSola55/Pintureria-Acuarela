namespace Pinturería_Acuarela
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Product_Business = new HashSet<Product_Business>();
            Product_Order = new HashSet<Product_Order>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string description { get; set; }

        public int id_brand { get; set; }

        public int? id_category { get; set; }

        public int? id_subcategory { get; set; }

        public int? id_capacity { get; set; }

        public int? id_color { get; set; }

        public int quantity { get; set; }

        public int? internal_code { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Capacity Capacity { get; set; }

        public virtual Category Category { get; set; }

        public virtual Color Color { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Business> Product_Business { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Order> Product_Order { get; set; }

        public virtual Subcategory Subcategory { get; set; }
    }
}
