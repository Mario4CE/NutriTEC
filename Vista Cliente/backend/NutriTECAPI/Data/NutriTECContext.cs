using Microsoft.EntityFrameworkCore;
using NutriTECAPI.Models;

namespace NutriTECAPI.Data
{
    /// <summary>
    /// DbContext para la aplicación NutriTEC
    /// Configura las entidades y sus relaciones
    /// </summary>
    public class NutriTECContext : DbContext
    {
        public NutriTECContext(DbContextOptions<NutriTECContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Medida> Medidas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Consumo> Consumos { get; set; }
        public DbSet<ConsumoProducto> ConsumoProductos { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<RecetaProducto> RecetaProductos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de Medida
            modelBuilder.Entity<Medida>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Medidas)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CodigoBarras).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.CodigoBarras).IsUnique();
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración de Consumo
            modelBuilder.Entity<Consumo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Consumos)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de ConsumoProducto
            modelBuilder.Entity<ConsumoProducto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Consumo)
                    .WithMany(c => c.ConsumoProductos)
                    .HasForeignKey(e => e.ConsumoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.ConsumoProductos)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Receta
            modelBuilder.Entity<Receta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Recetas)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de RecetaProducto
            modelBuilder.Entity<RecetaProducto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Receta)
                    .WithMany(r => r.RecetaProductos)
                    .HasForeignKey(e => e.RecetaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.RecetaProductos)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
