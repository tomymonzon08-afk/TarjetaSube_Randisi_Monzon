namespace TarjetaSube
{
    public record HistorialDeViajes(int ViajesHoy, DateTime? UltimoViaje, int ViajesDelMes = 0, string? LineaUltimoBoleto = null)
    {
        public static readonly HistorialDeViajes Vacio = new(0, null);
    }

}