namespace TarjetaSube
{
    public class ProveedorDeFechaManual : IProveedorDeFecha
    {
        private DateTime _ahora;

        public ProveedorDeFechaManual(DateTime inicio) => _ahora = inicio;

        public DateTime Ahora() => _ahora;

        public void Fijar(DateTime fecha) => _ahora = fecha;

        public void Avanzar(TimeSpan tiempo) => _ahora += tiempo;
    }
}