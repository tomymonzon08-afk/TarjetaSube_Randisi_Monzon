namespace TarjetaSube.Tests
{
    public class TarjetaTipoTests : BaseDeTests
    {
        [TestCase(100, 1580)]
        [TestCase(50, 790)]
        [TestCase(0, 0)]
        public void CalcularTarifa_AplicaElPorcentaje(int porcentaje, int esperado)
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = porcentaje };

            Assert.That(tipo.CalcularTarifa(Reglas.TarifaBasica), Is.EqualTo(esperado));
        }

        [Test]
        public void ModeloInicial_ExistenLosCuatroTipos()
        {
            Assert.That(Db.TarjetaTipos.Count(), Is.EqualTo(4));
            Assert.That(Db.TarjetaTipos.Find(2)!.PorcentajeTarifa, Is.EqualTo(50));
        }
    }
}