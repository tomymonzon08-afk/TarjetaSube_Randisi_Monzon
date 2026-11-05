namespace TarjetaSube.Tests
{
    public class ProveedorDeFechaManualTests
    {
        private static readonly DateTime Inicio = new(2026, 9, 21, 10, 0, 0);

        [Test]
        public void Ahora_DevuelveLaFechaInicial()
        {
            var reloj = new ProveedorDeFechaManual(Inicio);

            Assert.That(reloj.Ahora(), Is.EqualTo(Inicio));
        }

        [Test]
        public void Fijar_CambiaLaFechaActual()
        {
            var reloj = new ProveedorDeFechaManual(Inicio);
            var otra = new DateTime(2026, 9, 26, 12, 0, 0);

            reloj.Fijar(otra);

            Assert.That(reloj.Ahora(), Is.EqualTo(otra));
        }

        [Test]
        public void Avanzar_SumaElTiempoALaFechaActual()
        {
            var reloj = new ProveedorDeFechaManual(Inicio);

            reloj.Avanzar(TimeSpan.FromMinutes(90));

            Assert.That(reloj.Ahora(), Is.EqualTo(Inicio.AddMinutes(90)));
        }
    }
}