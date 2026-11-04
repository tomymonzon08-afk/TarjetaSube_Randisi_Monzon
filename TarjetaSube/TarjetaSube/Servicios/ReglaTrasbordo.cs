namespace TarjetaSube
{
    public static class ReglaTrasbordo
    {
        public static bool Aplica(DateTime fecha, string linea, HistorialDeViajes historial)
        {
            if (historial.UltimoViaje is null || historial.LineaUltimoBoleto is null)
                return false;
            if (!Reglas.FranjaTrasbordos.Contiene(fecha))
                return false;
            if (string.Equals(historial.LineaUltimoBoleto, linea, StringComparison.OrdinalIgnoreCase))
                return false;
            return fecha - historial.UltimoViaje.Value <= TimeSpan.FromMinutes(Reglas.MinutosVentanaTrasbordo);
        }
    }
}