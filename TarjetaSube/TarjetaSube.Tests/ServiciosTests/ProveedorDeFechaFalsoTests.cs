namespace TarjetaSube.Tests
{
    public class ProveedorDeFechaFalsoTests
    {
        private static readonly DateTime Inicio = new(2026, 9, 21, 10, 0, 0);

        [Test]
        public void Ahora_DevuelveLaFechaInicial()
        {
            var reloj = new ProveedorDeFechaFalso(Inicio);

            Assert.That(reloj.Ahora(), Is.EqualTo(Inicio));
        }

        [Test]
        public void Fecha_AlCambiarla_AhoraDevuelveLaNueva()
        {
            var reloj = new ProveedorDeFechaFalso(Inicio);
            var otra = new DateTime(2026, 9, 22, 8, 0, 0);

            reloj.Fecha = otra;

            Assert.That(reloj.Ahora(), Is.EqualTo(otra));
        }

        [Test]
        public void Avanzar_SumaElTiempoALaFechaActual()
        {
            var reloj = new ProveedorDeFechaFalso(Inicio);

            reloj.Avanzar(TimeSpan.FromMinutes(4));
            reloj.Avanzar(TimeSpan.FromMinutes(2));

            Assert.That(reloj.Ahora(), Is.EqualTo(Inicio.AddMinutes(6)));
        }
    }
}