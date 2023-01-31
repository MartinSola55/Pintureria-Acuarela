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

    public partial class Product_Business
    {
        public int id_product { get; set; }
        public int id_business { get; set; }
        [Range(0, 10000, ErrorMessage = "Debes agregar un stock mínimoentre 0 y 10.000")]
        public Nullable<int> minimum_stock { get; set; }
    
        public virtual Business Business { get; set; }
        public virtual Product Product { get; set; }
    }
}
