using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Pinturería_Acuarela
{
    public partial class EFModel : DbContext
    {
        public EFModel()
            : base("name=EFModel")
        {
        }

        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<Sucursal> Sucursal { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Producto>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursal>()
                .Property(e => e.city)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursal>()
                .Property(e => e.adress)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursal>()
                .HasMany(e => e.Producto)
                .WithRequired(e => e.Sucursal)
                .HasForeignKey(e => e.id_sucursal)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sucursal>()
                .HasMany(e => e.Usuario)
                .WithOptional(e => e.Sucursal)
                .HasForeignKey(e => e.id_sucursal);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.password)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
