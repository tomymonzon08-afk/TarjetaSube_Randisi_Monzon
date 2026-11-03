namespace TarjetaSube
{
    public record FranjaHoraria(DayOfWeek PrimerDia, DayOfWeek UltimoDia, int HoraDesde, int HoraHasta)
    {
        public bool Contiene(DateTime fecha) =>
            fecha.DayOfWeek >= PrimerDia && fecha.DayOfWeek <= UltimoDia
            && fecha.Hour >= HoraDesde && fecha.Hour < HoraHasta;
    }
}
