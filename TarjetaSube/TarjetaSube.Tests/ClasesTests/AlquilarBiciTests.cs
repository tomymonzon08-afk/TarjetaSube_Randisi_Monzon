namespace TarjetaSube.Tests
{
    public class AlquilerBiciTests
    {
        [Test]
        public void TotalAbonado_SumaLaTarifaYCadaMultaCobrada()
        {
            var alquiler = new AlquilerBici
            {
                TarifaCobrada = 2418.75m,
                MultasCobradas = 2
            };
            Assert.That(alquiler.TotalAbonado, Is.EqualTo(4918.75m));
        }

        [Test]
        public void TotalAbonado_SinCobroEsCero()
        {
            var alquiler = new AlquilerBici
            {
                TarifaCobrada = 0,
                MultasCobradas = 0
            };
            Assert.That(alquiler.TotalAbonado, Is.EqualTo(0));
        }
    }
}