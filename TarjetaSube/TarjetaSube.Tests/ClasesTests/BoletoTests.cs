namespace TarjetaSube.Tests
{
    public class BoletoTests : BaseDeTests
    {
        [TestCase("Normal", 100, 1580, 3420)]
        [TestCase("Medio boleto estudiantil", 50, 790, 4210)]
        [TestCase("Boleto gratuito estudiantil", 0, 0, 5000)]
        [TestCase("Franquicia completa", 0, 0, 5000)]
        public void PagarCon_BoletoInformaTipoLineaTotalYSaldo(string tipo, int porcentaje, int total, int saldoRestante)
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta
            {
                Saldo = 5000,
                TarjetaTipo = new TarjetaTipo { Nombre = tipo, PorcentajeTarifa = porcentaje }
            };

            colectivo.PagarCon(tarjeta, FechaFija, HistorialDeViajes.Vacio, out var boleto);

            Assert.That(boleto!.TipoTarjeta, Is.EqualTo(tipo));
            Assert.That(boleto.Linea, Is.EqualTo("K"));
            Assert.That(boleto.TotalAbonado, Is.EqualTo(total));
            Assert.That(boleto.SaldoRestante, Is.EqualTo(saldoRestante));
            Assert.That(boleto.FechaHora, Is.EqualTo(FechaFija));
        }

        [Test]
        public void PagarCon_ViajePlus_InformaSaldoNegativoYPlusAdeudado()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta { Saldo = 1000, TarjetaTipo = new TarjetaTipo { Nombre = "Normal", PorcentajeTarifa = 100 } };

            colectivo.PagarCon(tarjeta, FechaFija, HistorialDeViajes.Vacio, out var boleto);

            Assert.That(boleto!.TotalAbonado, Is.EqualTo(1580));
            Assert.That(boleto.SaldoRestante, Is.EqualTo(-580));
            Assert.That(boleto.PlusAdeudado, Is.EqualTo(580));
        }

        [Test]
        public void PlusAdeudado_ConSaldoPositivo_EsCero()
        {
            var boleto = new Boleto { SaldoRestante = 3420 };

            Assert.That(boleto.PlusAdeudado, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_ConSaldoPendiente_InformaElSaldoYaAcreditado()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta
            {
                Saldo = 56000,
                SaldoPendiente = 4000,
                TarjetaTipo = new TarjetaTipo { Nombre = "Normal", PorcentajeTarifa = 100 }
            };

            colectivo.PagarCon(tarjeta, FechaFija, HistorialDeViajes.Vacio, out var boleto);

            Assert.That(boleto!.SaldoRestante, Is.EqualTo(56000));
        }
    }
}