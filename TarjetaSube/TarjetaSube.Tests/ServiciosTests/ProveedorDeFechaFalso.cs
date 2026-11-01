namespace TarjetaSube.Tests
{
    public class ProveedorDeFechaFalso : IProveedorDeFecha
    {
        public DateTime Fecha { get; set; }

        public ProveedorDeFechaFalso(DateTime fecha) => Fecha = fecha;

        public DateTime Ahora() => Fecha;

        public void Avanzar(TimeSpan tiempo) => Fecha += tiempo;
    }
}
