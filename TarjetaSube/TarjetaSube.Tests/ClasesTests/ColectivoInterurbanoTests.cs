namespace TarjetaSube.Tests
{
    public class ColectivoInterurbanoTests
    {
        private static readonly DateTime Lunes10 = new(2026, 9, 21, 10, 0, 0);

        [Test]
        public void TarifaBase_Urbana_EsLaTarifaBasica()
        {
            Assert.That(new Colectivo { Linea = "K" }.TarifaBase,
            Is.EqualTo(Reglas.TarifaBasica));
        }
        [Test]
        public void TarifaBase_Interurbana_EsLaTarifaInterurbana()
        {
            Assert.That(new Colectivo { Linea = "Gálvez", EsInterurbana = true}.TarifaBase, Is.EqualTo(3000));
        }
        [Test]
        public void PagarCon_Interurbana_DescuentaYRecaudaLaTarifaInterurbana()
        {
            var colectivo = new Colectivo { Linea = "Gálvez", EsInterurbana = true };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = new TarjetaTipo {PorcentajeTarifa = 100 } };
            colectivo.PagarCon(tarjeta, Lunes10, HistorialDeViajes.Vacio, out var boleto);
            Assert.That(boleto!.Tarifa, Is.EqualTo(3000));
            Assert.That(tarjeta.Saldo, Is.EqualTo(2000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(3000));
        }
    }
}