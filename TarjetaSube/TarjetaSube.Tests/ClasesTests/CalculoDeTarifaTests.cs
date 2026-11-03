namespace TarjetaSube.Tests
{
    public class CalculoDeTarifaTests
    {
        [TestCase(100, 3000)]
        [TestCase(50, 1500)]
        [TestCase(0, 0)]
        public void CalcularTarifa_UsaLaTarifaBaseRecibida(int porcentaje, int esperado)
        {
            var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo { PorcentajeTarifa = porcentaje } };

            Assert.That(tarjeta.CalcularTarifa(0, 3000), Is.EqualTo(esperado));
        }
    }
}
