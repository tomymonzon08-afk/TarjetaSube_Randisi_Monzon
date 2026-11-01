using System;
using System.Collections.Generic;
using System.Text;

namespace TarjetaSube.Tests
{
    public class TarjetaTests : BaseDeTests
    {

        [TestCase(2000)]
        [TestCase(3000)]
        [TestCase(4000)]
        [TestCase(5000)]
        [TestCase(8000)]
        [TestCase(10000)]
        [TestCase(15000)]
        [TestCase(20000)]
        [TestCase(25000)]
        [TestCase(30000)]
        public void AgregarSaldo_MontoAceptado_SumaAlSaldo(int monto)
        {
            var tarjeta = new Tarjeta();

            tarjeta.AgregarSaldo(monto);

            Assert.That(tarjeta.Saldo, Is.EqualTo(monto));
        }

        [TestCase(0)]
        [TestCase(1000)]
        [TestCase(2500)]
        [TestCase(-2000)]
        [TestCase(35000)]
        public void AgregarSaldo_MontoNoAceptado_LanzaExcepcionYNoCambiaSaldo(int monto)
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            Assert.Throws<ArgumentException>(() => tarjeta.AgregarSaldo(monto));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void AgregarSaldo_LlegaExactoAlLimite_Permite()
        {
            var tarjeta = new Tarjeta { Saldo = 10000 };

            tarjeta.AgregarSaldo(30000);

            Assert.That(tarjeta.Saldo, Is.EqualTo(Reglas.SaldoMaximo));
        }

        [Test]
        public void AgregarSaldo_SuperaElLimite_LanzaExcepcionYNoCambiaSaldo()
        {
            var tarjeta = new Tarjeta { Saldo = 39000 };

            Assert.Throws<InvalidOperationException>(() => tarjeta.AgregarSaldo(2000));
            Assert.That(tarjeta.Saldo, Is.EqualTo(39000));
        }

        [Test]
        public void DescontarSaldo_SaldoSuficiente_Resta()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            tarjeta.DescontarSaldo(1580);

            Assert.That(tarjeta.Saldo, Is.EqualTo(3420));
        }

        [Test]
        public void DescontarSaldo_SaldoJusto_QuedaEnCero()
        {
            var tarjeta = new Tarjeta { Saldo = 1580 };

            tarjeta.DescontarSaldo(1580);

            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void DescontarSaldo_SaldoInsuficiente_LanzaExcepcionYNoQuedaNegativo()
        {
            var tarjeta = new Tarjeta { Saldo = -1000 };

            Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(1580));
            Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
        }

        [TestCase(-100)]
        public void DescontarSaldo_MontoNegativo_LanzaExcepcion(int monto)
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            Assert.Throws<ArgumentException>(() => tarjeta.DescontarSaldo(monto));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }
        
        [Test]
        public void CalcularTarifa_TarjetaNormal_EsLaTarifaBasica()
        {
            var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo { PorcentajeTarifa = 100 } };
            Assert.That(tarjeta.CalcularTarifa(), Is.EqualTo(Reglas.TarifaBasica));
        }
        [Test]
        public void PuedePagar_EnElLimiteExacto_EsTrue()
        {
            var tarjeta = new Tarjeta { Saldo = -420 };
            Assert.That(tarjeta.PuedePagar(1580), Is.True);
        }

        [Test]
        public void PuedePagar_UnPesoDebajoDelLimite_EsFalse()
        {
            var tarjeta = new Tarjeta { Saldo = -421 }; 
            Assert.That(tarjeta.PuedePagar(1580), Is.False);
        }

        [Test]
        public void AgregarSaldo_ConSaldoNegativo_DescuentaLaDeuda()
        {
            var tarjeta = new Tarjeta { Saldo = -580 };
            tarjeta.AgregarSaldo(5000);
            Assert.That(tarjeta.Saldo, Is.EqualTo(4420));
        }
    }
}
