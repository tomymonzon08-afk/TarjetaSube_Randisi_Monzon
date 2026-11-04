namespace TarjetaSube
{
    public record HistorialBici(bool TieneBiciEnUso, bool YaPagoHoy)
    {
        public static readonly HistorialBici Vacio = new(false, false);
    }
}