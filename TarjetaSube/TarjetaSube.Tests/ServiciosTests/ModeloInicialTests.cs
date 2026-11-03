namespace TarjetaSube.Tests
{
    public class ModeloInicialTests : BaseDeTests
    {
        [TestCase(1, true, false)]
        [TestCase(2, false, true)]
        [TestCase(3, false, true)]
        [TestCase(4, false, true)]
        public void TiposIniciales_TienenLosIndicadoresCorrectos(int id, bool usoFrecuente, bool franja)
        {
            var tipo = Db.TarjetaTipos.Find(id)!;

            Assert.That(tipo.AplicaUsoFrecuente, Is.EqualTo(usoFrecuente));
            Assert.That(tipo.UsaFranjaHoraria, Is.EqualTo(franja));
        }
    }
}
