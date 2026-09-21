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

            // Configuración de Colectivo
            modelBuilder.Entity<Colectivo>(entity =>
            {
                entity.ToTable("Colectivo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Linea).HasColumnName("linea").IsRequired();
            });

            // Configuración de Tarjeta
            modelBuilder.Entity<Tarjeta>(entity =>
            {
                entity.ToTable("Tarjeta");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
                entity.HasIndex(e => e.Numero).IsUnique(); // UNIQUE

                entity.Property(e => e.Saldo).HasColumnName("saldo").HasDefaultValue(0);
                entity.Property(e => e.Dueno).HasColumnName("dueno").IsRequired();

                // CHECK (saldo >= 0 AND saldo  t.HasCheckConstraint("CK_Tarjeta_Saldo", "saldo >= 0 AND saldo <= 40000"));
            });

            // Configuración de Boleto
            modelBuilder.Entity<Boleto>(entity =>
            {
                entity.ToTable("Boleto");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

                // DEFAULT CURRENT_TIMESTAMP mapeado a DateTime
                entity.Property(e => e.FechaHora).HasColumnName("fecha_hora").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Tarifa).HasColumnName("tarifa").HasDefaultValue(1580);
                entity.Property(e => e.TarjetaId).HasColumnName("tarjeta_id");
                entity.Property(e => e.ColectivoId).HasColumnName("colectivo_id");

                // Relación: Tarjeta (1) -> Boletos (N)
                entity.HasOne(d => d.TarjetaId)
                      .WithMany(p => p.Boletos)
                      .HasForeignKey(d => d.TarjetaId)
                      .OnDelete(DeleteBehavior.Restrict); // Evita borrado en cascada accidental

                // Relación: Colectivo (1) -> Boletos (N)
                entity.HasOne(d => d.ColectivoId)
                      .WithMany(p => p.Boletos)
                      .HasForeignKey(d => d.ColectivoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}