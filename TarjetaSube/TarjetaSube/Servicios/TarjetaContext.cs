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
        public DbSet<EstacionBici> EstacionesBici => Set<EstacionBici>();
        public DbSet<AlquilerBici> AlquileresBici => Set<AlquilerBici>();


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
                entity.Property(e => e.EsInterurbana).HasColumnName("es_interurbana");
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

                entity.Property(e => e.MultasBiciPendientes).HasColumnName("multas_bici_pendientes");

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
                entity.Property(e => e.EsTransbordo).HasColumnName("es_transbordo");
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
                entity.Property(e => e.UsaFranjaHoraria).HasColumnName("usa_franja_horaria");
                entity.Property(e => e.AplicaUsoFrecuente).HasColumnName("aplica_uso_frecuente");
                entity.HasData(
                    new TarjetaTipo { Id = 1, Nombre = "Normal", PorcentajeTarifa = 100, AplicaUsoFrecuente = true },
                    new TarjetaTipo { Id = 2, Nombre = "Medio boleto estudiantil", PorcentajeTarifa = 50, MinutosEntreViajes = 5, ViajesConBeneficioPorDia = 2, UsaFranjaHoraria = true },
                    new TarjetaTipo { Id = 3, Nombre = "Boleto gratuito estudiantil", PorcentajeTarifa = 0, ViajesConBeneficioPorDia = 2, UsaFranjaHoraria = true },
                    new TarjetaTipo { Id = 4, Nombre = "Franquicia completa", PorcentajeTarifa = 0, UsaFranjaHoraria = true });
            });
            modelBuilder.Entity<EstacionBici>(entity =>
            {
                entity.ToTable("EstacionBici");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired();
                entity.Property(e => e.Recaudacion).HasColumnName("recaudacion");
            });

            modelBuilder.Entity<AlquilerBici>(entity =>
            {
                entity.ToTable("AlquilerBici");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.FechaRetiro).HasColumnName("fecha_retiro");
                entity.Property(e => e.FechaDevolucion).HasColumnName("fecha_devolucion");
                entity.Property(e => e.PagoElDia).HasColumnName("pago_el_dia");
                entity.Property(e => e.ExcedioElTiempo).HasColumnName("excedio_el_tiempo");
                entity.Property(e => e.TarifaCobrada).HasColumnName("tarifa_cobrada");
                entity.Property(e => e.MultasCobradas).HasColumnName("multas_cobradas");
                entity.Property(e => e.TarjetaId).HasColumnName("tarjeta_id");

                entity.Property(e => e.EstacionId).HasColumnName("estacion_id");
                entity.HasOne(d => d.Tarjeta)
                    .WithMany()
                    .HasForeignKey(d => d.TarjetaId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.Estacion)
                    .WithMany(p => p.Alquileres)
                    .HasForeignKey(d => d.EstacionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        

    }
}