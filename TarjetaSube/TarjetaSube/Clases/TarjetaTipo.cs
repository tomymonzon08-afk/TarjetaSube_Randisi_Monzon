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

        public int CalcularTarifa(int tarifaBase, int viajesHoy = 0)
        {
            if (ViajesConBeneficioPorDia.HasValue && viajesHoy >= ViajesConBeneficioPorDia.Value)
                return tarifaBase;
            return tarifaBase * PorcentajeTarifa / 100;
        }

        public bool PermiteViajar(DateTime ahora, DateTime? ultimoViaje)
        {
            if (MinutosEntreViajes <= 0 || ultimoViaje is null)
                return true;
            return ahora - ultimoViaje.Value >= TimeSpan.FromMinutes(MinutosEntreViajes);
        }
    }
}