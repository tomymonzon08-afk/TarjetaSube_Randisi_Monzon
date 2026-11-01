

namespace TarjetaSube.Tests
{
    public class ColectivoTests : BaseDeTests
    {
        [Test]
        public void PagarCon_SinBaseDeDatos_DescuentaYArmaElBoleto()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = new TarjetaTipo { PorcentajeTarifa = 100 } };

            var resultado = colectivo.PagarCon(tarjeta, FechaFija, HistorialDeViajes.Vacio, out var boleto);

            Assert.That(resultado, Is.True);
            Assert.That(boleto!.Tarifa, Is.EqualTo(1580));
            Assert.That(tarjeta.Saldo, Is.EqualTo(3420));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(1580));
        }

        [Test]
        public void PagarCon_SaldoInsuficiente_DevuelveFalseYNoCambiaNada()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta { Saldo = -1000, TarjetaTipo = new TarjetaTipo { PorcentajeTarifa = 100 } };

            var resultado = colectivo.PagarCon(tarjeta, FechaFija, HistorialDeViajes.Vacio, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_ViajeAntesDelIntervalo_DevuelveFalseYNoCambiaNada()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tipo = new TarjetaTipo { PorcentajeTarifa = 50, MinutosEntreViajes = 5, ViajesConBeneficioPorDia = 2 };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = tipo };
            var historial = new HistorialDeViajes(1, FechaFija.AddMinutes(-4));

            var resultado = colectivo.PagarCon(tarjeta, FechaFija, historial, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_TipoNuevoDefinidoSoloPorDatos_AplicaSuPorcentaje()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = new TarjetaTipo { Nombre = "Jubilado", PorcentajeTarifa = 25 } };

            colectivo.PagarCon(tarjeta, FechaFija, HistorialDeViajes.Vacio, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(395));
        }
    }
}
