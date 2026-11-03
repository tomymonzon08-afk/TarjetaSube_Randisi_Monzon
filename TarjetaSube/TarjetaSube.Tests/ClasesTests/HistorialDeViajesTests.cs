namespace TarjetaSube.Tests
{
    public class HistorialDeViajesTests : BaseDeTests
    {
        [Test]
        public void Vacio_NoTieneViajesNiUltimoViaje()
        {
            var historial = HistorialDeViajes.Vacio;

            Assert.That(historial.ViajesHoy, Is.EqualTo(0));
            Assert.That(historial.UltimoViaje, Is.Null);
        }
        [Test]
        public void Constructor_LosParametrosOpcionalesTienenValoresPorDefecto()
        {
            var historial = new HistorialDeViajes(2, FechaFija);

            Assert.That(historial.ViajesDelMes, Is.EqualTo(0));
            Assert.That(historial.LineaUltimoBoleto, Is.Null);
        }

    }
}