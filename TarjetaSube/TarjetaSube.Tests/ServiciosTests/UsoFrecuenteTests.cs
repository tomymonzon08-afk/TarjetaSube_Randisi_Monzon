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

        // viajesDelMes = viajes anteriores; el que se paga es el número viajesDelMes + 1
        [TestCase(0, Reglas.TarifaBasica)]
        [TestCase(28, Reglas.TarifaBasica)]                  // viaje 29
        [TestCase(29, Reglas.TarifaBasica * 80 / 100)]       // viaje 30
        [TestCase(78, Reglas.TarifaBasica * 80 / 100)]       // viaje 79
        [TestCase(79, Reglas.TarifaBasica * 75 / 100)]       // viaje 80: 1612,5 se trunca a 1612
        [TestCase(150, Reglas.TarifaBasica * 75 / 100)]
        public void CalcularTarifa_TipoNormal_AplicaElTramoDelMes(int viajesDelMes, int esperado)
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 100, AplicaUsoFrecuente = true };

            Assert.That(tipo.CalcularTarifa(Reglas.TarifaBasica, 0, viajesDelMes), Is.EqualTo(esperado));
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
