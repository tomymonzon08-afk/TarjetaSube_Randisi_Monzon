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
        
        [TestCase(0, 790)]
        [TestCase(1, 790)]
        [TestCase(2, 1580)]
        [TestCase(5, 1580)]
        public void CalcularTarifa_ConLimiteDeViajes_CobraNormalDespuesDelLimite(int viajesHoy, int esperado)
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 50, ViajesConBeneficioPorDia = 2 };
            Assert.That(tipo.CalcularTarifa(Reglas.TarifaBasica, viajesHoy), Is.EqualTo(esperado));
        }

        [Test]
        public void CalcularTarifa_SinLimite_AplicaSiempreElPorcentaje()
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 0, ViajesConBeneficioPorDia = null };
            Assert.That(tipo.CalcularTarifa(Reglas.TarifaBasica, 50), Is.EqualTo(0));
        }

        [TestCase(4, false)]
        [TestCase(5, true)]
        [TestCase(30, true)]
        public void PermiteViajar_SegunLosMinutosTranscurridos(int minutos, bool esperado)
        {
            var tipo = new TarjetaTipo { MinutosEntreViajes = 5 };
            var ultimo = new DateTime(2026, 9, 21, 10, 0, 0);

            Assert.That(tipo.PermiteViajar(ultimo.AddMinutes(minutos), ultimo), Is.EqualTo(esperado));
        }

        [Test]
        public void PermiteViajar_SinLimiteODeSinViajesPrevios_EsTrue()
        {
            var ahora = new DateTime(2026, 9, 21, 10, 0, 0);
            Assert.That(new TarjetaTipo { MinutosEntreViajes = 0 }.PermiteViajar(ahora, ahora), Is.True);
            Assert.That(new TarjetaTipo { MinutosEntreViajes = 5 }.PermiteViajar(ahora, null), Is.True);
        }

    }
}