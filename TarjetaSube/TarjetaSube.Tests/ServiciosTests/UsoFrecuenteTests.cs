namespace TarjetaSube.Tests
{
    public class UsoFrecuenteTests
    {
        [TestCase(1, 0)]
        [TestCase(29, 0)]
        [TestCase(30, 20)]
        [TestCase(79, 20)]
        [TestCase(80, 25)]
        [TestCase(200, 25)]
        public void PorcentajeDescuento_SegunElNumeroDeViaje(int numeroDeViaje, int esperado)
        {
            Assert.That(UsoFrecuente.PorcentajeDescuento(numeroDeViaje), Is.EqualTo(esperado));
        }

        [TestCase(0, 100)]
        [TestCase(28, 100)]    // viaje 29
        [TestCase(29, 80)]     // viaje 30
        [TestCase(78, 80)]     // viaje 79
        [TestCase(79, 75)]     // viaje 80: 2150 * 75% = 1612,50
        [TestCase(150, 75)]
        public void CalcularTarifa_TipoNormal_AplicaElTramoDelMes(int viajesDelMes, int porcentajePagado)
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 100, AplicaUsoFrecuente = true };

            Assert.That(tipo.CalcularTarifa(Reglas.TarifaBasica, 0, viajesDelMes),
                Is.EqualTo(Reglas.TarifaBasica * porcentajePagado / 100m));
        }

        [Test]
        public void CalcularTarifa_TipoSinUsoFrecuente_NoRecibeDescuento()
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 50, AplicaUsoFrecuente = false };

            Assert.That(tipo.CalcularTarifa(Reglas.TarifaBasica, 0, 100), Is.EqualTo(Reglas.TarifaBasica / 2));
        }

        [TestCase(29, Reglas.TarifaInterurbana * 80 / 100)]
        [TestCase(79, Reglas.TarifaInterurbana * 75 / 100)]
        public void CalcularTarifa_SobreLaTarifaInterurbana_AplicaElDescuento(int viajesDelMes, int esperado)
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 100, AplicaUsoFrecuente = true };

            Assert.That(tipo.CalcularTarifa(Reglas.TarifaInterurbana, 0, viajesDelMes), Is.EqualTo(esperado));
        }
    }
}
