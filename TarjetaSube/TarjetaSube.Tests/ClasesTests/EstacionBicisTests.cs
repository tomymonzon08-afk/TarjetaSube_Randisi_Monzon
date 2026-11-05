namespace TarjetaSube.Tests
{
    public class EstacionBiciTests
    {
        private static readonly DateTime Lunes10 = new(2026, 9, 21, 10, 0, 0);

        [Test]
        public void RetirarCon_SaldoSuficiente_CobraLaTarifaDelDia()
        {
            var estacion = new EstacionBici { Nombre = "Centro" };
            var tarjeta = new Tarjeta { Saldo = 5000 };

            var resultado = estacion.RetirarCon(tarjeta, Lunes10, HistorialBici.Vacio, out var alquiler);

            Assert.That(resultado, Is.True);
            Assert.That(alquiler!.TarifaCobrada, Is.EqualTo(2418.75m));
            Assert.That(alquiler.PagoElDia, Is.True);
            Assert.That(tarjeta.Saldo, Is.EqualTo(2581.25m));
            Assert.That(estacion.Recaudacion, Is.EqualTo(2418.75m));
        }

        [Test]
        public void RetirarCon_SaldoJusto_PermiteElRetiroYQuedaEnCero()
        {
            var tarjeta = new Tarjeta { Saldo = 2418.75m };

            Assert.That(new EstacionBici().RetirarCon(tarjeta, Lunes10, HistorialBici.Vacio, out _), Is.True);
            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void RetirarCon_SaldoInsuficiente_SeRechazaYNoDejaRastros()
        {
            var estacion = new EstacionBici();
            var tarjeta = new Tarjeta { Saldo = 2418.74m };

            var resultado = estacion.RetirarCon(tarjeta, Lunes10, HistorialBici.Vacio, out var alquiler);

            Assert.That(resultado, Is.False);
            Assert.That(alquiler, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(2418.74m));
            Assert.That(estacion.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void RetirarCon_ConMultas_CobraTarifaMasMultasYLasSaldan()
        {
            var estacion = new EstacionBici();
            var tarjeta = new Tarjeta { Saldo = 10000, MultasBiciPendientes = 2 };

            estacion.RetirarCon(tarjeta, Lunes10, HistorialBici.Vacio, out var alquiler);

            Assert.That(alquiler!.MultasCobradas, Is.EqualTo(2));
            Assert.That(alquiler.TotalAbonado, Is.EqualTo(4918.75m));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5081.25m));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
            Assert.That(estacion.Recaudacion, Is.EqualTo(4918.75m));
        }

        [Test]
        public void RetirarCon_AlcanzaParaLaTarifaPeroNoParaLaMulta_SeRechazaYLaMultaSigue()
        {
            var tarjeta = new Tarjeta { Saldo = 3000, MultasBiciPendientes = 1 };   // la tarifa + multa son 3668,75

            var resultado = new EstacionBici().RetirarCon(tarjeta, Lunes10, HistorialBici.Vacio, out _);

            Assert.That(resultado, Is.False);
            Assert.That(tarjeta.Saldo, Is.EqualTo(3000));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1));
        }

        [Test]
        public void RetirarCon_YaPagoHoy_NoCobraYLasMultasSiguenPendientes()
        {
            var tarjeta = new Tarjeta { Saldo = 5000, MultasBiciPendientes = 1 };

            var resultado = new EstacionBici().RetirarCon(tarjeta, Lunes10, new HistorialBici(false, true), out var alquiler);

            Assert.That(resultado, Is.True);
            Assert.That(alquiler!.TarifaCobrada, Is.EqualTo(0));
            Assert.That(alquiler.PagoElDia, Is.False);
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1));
        }

        [Test]
        public void RetirarCon_ConBiciEnUso_SeRechaza()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            Assert.That(new EstacionBici().RetirarCon(tarjeta, Lunes10, new HistorialBici(true, false), out _), Is.False);
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }

        [TestCase(100)]
        [TestCase(50)]
        [TestCase(0)]
        public void RetirarCon_CualquierTipoDeTarjeta_PagaLoMismo(int porcentajeTarifa)
        {
            var tipo = new TarjetaTipo { PorcentajeTarifa = porcentajeTarifa };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = tipo };

            new EstacionBici().RetirarCon(tarjeta, Lunes10, HistorialBici.Vacio, out var alquiler);

            Assert.That(alquiler!.TarifaCobrada, Is.EqualTo(Reglas.TarifaBici));
        }
    }
}
