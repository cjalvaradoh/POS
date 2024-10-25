using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Models;

namespace UspgPOS.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Definir las tablas en DbSet
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Clasificacion> Clasificacion { get; set; }
        public DbSet<DetalleVenta> DetallesDeVenta { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
