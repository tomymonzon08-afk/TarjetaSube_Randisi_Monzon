namespace TarjetaSube
{
    public class TarjetaTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PorcentajeTarifa { get; set; }
        public List<Tarjeta> Tarjetas { get; set; } = new();
        public int MinutosEntreViajes { get; set; }
        public int? ViajesConBeneficioPorDia { get; set; }

        public bool UsaFranjaHoraria { get; set; }
        public bool AplicaUsoFrecuente { get; set; }

        public decimal CalcularTarifa(decimal tarifaBase, int viajesHoy = 0, int viajesDelMes = 0)
        {
            decimal tarifa = tarifaBase * PorcentajeTarifa / 100;

            if (ViajesConBeneficioPorDia.HasValue && viajesHoy >= ViajesConBeneficioPorDia.Value)
                tarifa = tarifaBase;

            if (AplicaUsoFrecuente)
                tarifa = tarifa * (100 - UsoFrecuente.PorcentajeDescuento(viajesDelMes + 1)) / 100;

            return tarifa;
        }



        public bool PermiteViajar(DateTime ahora, DateTime? ultimoViaje)
        {
            if (UsaFranjaHoraria && !Reglas.FranjaFranquicias.Contiene(ahora))
                return false;
            if (MinutosEntreViajes <= 0 || ultimoViaje is null)
                return true;
            return ahora - ultimoViaje.Value >= TimeSpan.FromMinutes(MinutosEntreViajes);
        }
    }
}