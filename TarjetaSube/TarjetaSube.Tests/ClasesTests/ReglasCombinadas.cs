namespace TarjetaSube.Tests
{
    public class ReglasCombinadasTests
    {
        private static readonly DateTime Lunes10 = new(2026, 9, 21, 10, 0, 0);
        private static readonly DateTime Sabado12 = new(2026, 9, 26, 12, 0, 0);
        private static readonly DateTime Domingo12 = new(2026, 9, 27, 12, 0, 0);

        private static Tarjeta Normal(decimal saldo) => new()
        {
            Saldo = saldo,
            TarjetaTipo = new TarjetaTipo
            {
                Nombre = "Normal",
                PorcentajeTarifa =
            100,
                AplicaUsoFrecuente = true
            }
        };

        private static Tarjeta MedioBoleto(decimal saldo) => new()
        {
            Saldo = saldo,
            TarjetaTipo = new TarjetaTipo
            {
                Nombre = "Medio boleto estudiantil",
                PorcentajeTarifa = 50,
                MinutosEntreViajes = 5,
                ViajesConBeneficioPorDia = 2,
                UsaFranjaHoraria = true
            }
        };

        [Test]
        public void
        PagarCon_MedioBoletoEnLineaInterurbana_PagaLaMitadDeLaTarifaInterurbana()
        {
            var colectivo = new Colectivo { Linea = "Gálvez", EsInterurbana = true };
            colectivo.PagarCon(MedioBoleto(5000), Lunes10, HistorialDeViajes.Vacio,
            out var boleto);
            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaInterurbana / 2m));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(Reglas.TarifaInterurbana /
            2m));
        }

        [Test]
        public void
        PagarCon_UsoFrecuenteEnLineaInterurbana_DescuentaSobreLaTarifaInterurbana()
        {
            var colectivo = new Colectivo { Linea = "Gálvez", EsInterurbana = true };
            var historial = new HistorialDeViajes(0, null, 29); 
            colectivo.PagarCon(Normal(5000), Lunes10, historial, out var boleto);
            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaInterurbana * 80 /
            100m));
        }

        [Test]
        public void PagarCon_MedioBoletoConMuchosViajesDelMes_NoRecibeUsoFrecuente()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var historial = new HistorialDeViajes(0, null, 79);
            colectivo.PagarCon(MedioBoleto(5000), Lunes10, historial, out var
            boleto);
            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica / 2m));
        }

        [Test]
        public void
        PagarCon_FranquiciaInterurbanaFueraDeLaFranja_SeRechazaYNoCambiaNada()
        {
            var colectivo = new Colectivo { Linea = "Gálvez", EsInterurbana = true };
            var tarjeta = MedioBoleto(5000);
            var resultado = colectivo.PagarCon(tarjeta, Sabado12,
            HistorialDeViajes.Vacio, out var boleto);
            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_TarjetaNormalElSabado_PuedeHacerTrasbordo()
        {
            var colectivo = new Colectivo { Linea = "101" };
            var tarjeta = Normal(5000);
            var historial = new HistorialDeViajes(1, Sabado12.AddMinutes(-30), 1,
            "K");
            colectivo.PagarCon(tarjeta, Sabado12, historial, out var boleto);
            Assert.That(boleto!.EsTransbordo, Is.True);
            Assert.That(boleto.Tarifa, Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void PagarCon_TarjetaNormalElDomingo_PagaYNoHayTrasbordo()
        {
            var colectivo = new Colectivo { Linea = "101" };
            var tarjeta = Normal(5000);
            var historial = new HistorialDeViajes(1, Domingo12.AddMinutes(-30), 1,
            "K");
            colectivo.PagarCon(tarjeta, Domingo12, historial, out var boleto);
            Assert.That(boleto!.EsTransbordo, Is.False);
            Assert.That(boleto.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica));
        }
    }
}