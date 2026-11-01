using System;
using System.Collections.Generic;
using System.Text;

namespace TarjetaSube.Tests
{
    public class ColectivoTests : BaseDeTests
    {
        [Test]
        public void PagarCon_SinBaseDeDatos_DescuentaYArmaElBoleto()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = new TarjetaTipo { PorcentajeTarifa = 100 } };

            var resultado = colectivo.PagarCon(tarjeta, FechaFija, out var boleto);

            Assert.That(resultado, Is.True);
            Assert.That(boleto!.Tarifa, Is.EqualTo(1580));
            Assert.That(tarjeta.Saldo, Is.EqualTo(3420));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(1580));
        }
    }
}
