namespace TarjetaSube.Tests
{
    public class ReglasBiciTests
    {
        [Test]
        public void TarifaBici_Es2418Con75() => Assert.That(Reglas.TarifaBici,
        Is.EqualTo(2418.75m));

        [Test]
        public void MultaBici_Es1250() => Assert.That(Reglas.MultaBici,
        Is.EqualTo(1250m));

        [Test]
        public void MinutosMaximosBici_EsUnaHora() =>
        Assert.That(Reglas.MinutosMaximosBici, Is.EqualTo(60));
    }
}