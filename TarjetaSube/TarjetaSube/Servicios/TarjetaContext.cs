using Microsoft.EntityFrameworkCore;

namespace TarjetaSube
{

    public class TarjetaContext : DbContext
    {
        public TarjetaContext() { }
        public TarjetaContext(DbContextOptions<TarjetaContext> options) : base(options) { }
        public DbSet<Colectivo> Colectivos => Set<Colectivo>();
        public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
        public DbSet<Boleto> Boletos => Set<Boleto>();
        public DbSet<TarjetaTipo> TarjetaTipos => Set<TarjetaTipo>();


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

                entity.Property(e => e.TarjetaTipoId).HasColumnName("tarjeta_tipo_id");
                entity.HasOne(d => d.TarjetaTipo)
                    .WithMany(p => p.Tarjetas)
                    .HasForeignKey(d => d.TarjetaTipoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.SaldoPendiente).HasColumnName("saldo_pendiente");

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

                entity.Property(e => e.TipoTarjeta).HasColumnName("tipo_tarjeta").IsRequired();
                entity.Property(e => e.Linea).HasColumnName("linea").IsRequired();
                entity.Property(e => e.SaldoRestante).HasColumnName("saldo_restante");

            });

            modelBuilder.Entity<TarjetaTipo>(entity =>
            {
                entity.ToTable("TarjetaTipo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired();
                entity.Property(e => e.PorcentajeTarifa).HasColumnName("porcentaje_tarifa");
                entity.Property(e => e.MinutosEntreViajes).HasColumnName("minutos_entre_viajes");
                entity.Property(e => e.ViajesConBeneficioPorDia).HasColumnName("viajes_con_beneficio_por_dia");
                entity.HasData(
                    new TarjetaTipo { Id = 1, Nombre = "Normal", PorcentajeTarifa = 100 },
                    new TarjetaTipo { Id = 2, Nombre = "Medio boleto estudiantil", PorcentajeTarifa = 50, MinutosEntreViajes = 5, ViajesConBeneficioPorDia = 2 },
                    new TarjetaTipo { Id = 3, Nombre = "Boleto gratuito estudiantil", PorcentajeTarifa = 0, ViajesConBeneficioPorDia = 2 },
                    new TarjetaTipo { Id = 4, Nombre = "Franquicia completa", PorcentajeTarifa = 0 });
            });
        }

    }
}