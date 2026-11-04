namespace TarjetaSube.Tests
{
    public class ReglasTests
    {
        [Test]
        public void TarifaBasica_EsLaVigente()
        {
        Assert.That(Reglas.TarifaBasica, Is.EqualTo(2150));
        }
    }
}