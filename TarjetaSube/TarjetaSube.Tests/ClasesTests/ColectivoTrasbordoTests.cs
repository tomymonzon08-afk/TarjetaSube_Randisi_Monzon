namespace TarjetaSube.Tests
{
    public class ColectivoTrasbordoTests
    {
        private static readonly DateTime Lunes10 = new(2026, 9, 21, 10, 0, 0);
        private static Tarjeta Normal(int saldo) => new() { Saldo = saldo, TarjetaTipo = new TarjetaTipo { Nombre = "Normal",PorcentajeTarifa = 100 } };
        [Test]
        public void PagarCon_Trasbordo_EsGratisYSeIndicaEnElBoleto()
        {
            var colectivo = new Colectivo { Linea = "101" };
            var tarjeta = Normal(5000);
            var historial = new HistorialDeViajes(1, Lunes10.AddMinutes(-30), 1, "K");
            var resultado = colectivo.PagarCon(tarjeta, Lunes10, historial, out var boleto);
            Assert.That(resultado, Is.True);
            Assert.That(boleto!.EsTransbordo, Is.True);
            Assert.That(boleto.Tarifa, Is.EqualTo(0));

            Assert.That(boleto.Linea, Is.EqualTo("101"));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }
        [Test]
        public void PagarCon_SinTrasbordo_NoMarcaElBoleto()
        {
            var colectivo = new Colectivo { Linea = "K" };
            colectivo.PagarCon(Normal(5000), Lunes10, HistorialDeViajes.Vacio, out var boleto);
            Assert.That(boleto!.EsTransbordo, Is.False);
        }
        [Test]
        public void PagarCon_TrasbordoConSaldoMinimo_SePermite()
        {
            var colectivo = new Colectivo { Linea = "101" };
            var tarjeta = Normal(Reglas.SaldoMinimo);
            var historial = new HistorialDeViajes(1, Lunes10.AddMinutes(-30), 1, "K");
            Assert.That(colectivo.PagarCon(tarjeta, Lunes10, historial, out _), Is.True);
            Assert.That(tarjeta.Saldo, Is.EqualTo(Reglas.SaldoMinimo));
        }
        [Test]
        public void
        PagarCon_MedioBoleto_ElTrasbordoNoChocaConLosCincoMinutosPeroLaMismaLineaSi()
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = 50, MinutosEntreViajes = 5, ViajesConBeneficioPorDia = 2 };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = tipo };
            var historial = new HistorialDeViajes(1, Lunes10.AddMinutes(-2), 1, "K");
            Assert.That(new Colectivo { Linea = "101" }.PagarCon(tarjeta, Lunes10,
            historial, out _), Is.True);
            Assert.That(new Colectivo { Linea = "K" }.PagarCon(tarjeta, Lunes10,
            historial, out _), Is.False);
        }
    }
}