namespace TarjetaSube.Tests
{
    public class HistorialDeViajesTests
    {
        [Test]
        public void Vacio_NoTieneViajesNiUltimoViaje()
        {
            var historial = HistorialDeViajes.Vacio;

            Assert.That(historial.ViajesHoy, Is.EqualTo(0));
            Assert.That(historial.UltimoViaje, Is.Null);
        }
    }
}