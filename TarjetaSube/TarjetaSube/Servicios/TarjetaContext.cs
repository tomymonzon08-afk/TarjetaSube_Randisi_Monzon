using Microsoft.EntityFrameworkCore;

namespace TarjetaSube
{

    public class TarjetaContext : DbContext
    {
        public DbSet<Colectivo> Colectivos => Set<Colectivo>();
        public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
        public DbSet<Boleto> Boletos => Set<Boleto>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=DBTarjetaSube.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Colectivo>(entity =>
            {
                entity.ToTable("Colectivo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Linea).HasColumnName("linea").IsRequired();
                entity.Property(e => e.Recaudacion).HasColumnName("recaudacion");
            });

            modelBuilder.Entity<Tarjeta>(entity =>
            {
                entity.ToTable("Tarjeta");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
                entity.HasIndex(e => e.Numero).IsUnique(); 

                entity.Property(e => e.Saldo).HasColumnName("saldo");
                entity.Property(e => e.Dueno).HasColumnName("dueno").IsRequired();

            });

            modelBuilder.Entity<Boleto>(entity =>
            {
                entity.ToTable("Boleto");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(e => e.FechaHora).HasColumnName("fecha_hora");

                entity.Property(e => e.Tarifa).HasColumnName("tarifa");
                entity.Property(e => e.TarjetaId).HasColumnName("tarjeta_id");
                entity.Property(e => e.ColectivoId).HasColumnName("colectivo_id");

                entity.HasOne(d => d.Tarjeta)
                .WithMany(p => p.Boletos)
                .HasForeignKey(d => d.TarjetaId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Colectivo)
                .WithMany(p => p.Boletos)
                .HasForeignKey(d => d.ColectivoId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}