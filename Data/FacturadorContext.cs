//esta clase es el puente entre el codigo y la base de datos
using Microsoft.EntityFrameworkCore;
using FacturadorARCA.Models;

namespace FacturadorARCA.Data
{
    //facturadorcontext hereda de dbcontext
    public class FacturadorContext : DbContext
    {
        //representa una tabla en la base de datos
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<ItemFactura> ItemsFactura { get; set; }

        //configuracion de la conexion a la base de datos
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=FacturadorDB;Trusted_Connection=true;TrustServerCertificate=true;"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configuracion de como se crean las tablas
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.RazonSocial).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CuilCuit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Domicilio).IsRequired().HasMaxLength(300);
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.HasKey(e => e.IdFactura);
                entity.Property(e => e.NumeroFactura).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(1);
                entity.Property(e => e.Fecha).IsRequired();

                entity.HasOne(f => f.Cliente)
                      .WithMany(c => c.Facturas)
                      .HasForeignKey(f => f.IdCliente)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ItemFactura>(entity =>
            {
                entity.HasKey(e => e.IdItem);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.Importe).IsRequired().HasColumnType("decimal(18,2)");

                entity.HasOne(i => i.Factura)
                      .WithMany(f => f.Items)
                      .HasForeignKey(i => i.IdFactura)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}