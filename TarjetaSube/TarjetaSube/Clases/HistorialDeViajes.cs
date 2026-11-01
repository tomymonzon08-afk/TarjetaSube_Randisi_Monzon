namespace TarjetaSube
{
    public record HistorialDeViajes(int ViajesHoy, DateTime? UltimoViaje)
    {
        public static readonly HistorialDeViajes Vacio = new(0, null);
    }
}