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
            var tarjeta = new Tarjeta { Saldo = 26000 };

            tarjeta.AgregarSaldo(30000);

            Assert.That(tarjeta.Saldo, Is.EqualTo(Reglas.SaldoMaximo));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
        }


        [Test]
        public void AgregarSaldo_SuperaElMaximo_AcreditaHastaElMaximoYDejaElExcedentePendiente()
        {
            var tarjeta = new Tarjeta { Saldo = 50000 };

            tarjeta.AgregarSaldo(10000);

            Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000));
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

        [Test]
        public void AcreditarCarga_ConEspacio_AcreditaTodoElPendiente()
        {
            var tarjeta = new Tarjeta { Saldo = 10000, SaldoPendiente = 3000 };

            tarjeta.AcreditarCarga();

            Assert.That(tarjeta.Saldo, Is.EqualTo(13000));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
        }

        [Test]
        public void AcreditarCarga_ConEspacioParcial_AcreditaSoloLoQueEntra()
        {
            var tarjeta = new Tarjeta { Saldo = 54420, SaldoPendiente = 4000 };

            tarjeta.AcreditarCarga();

            Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2420));
        }

        [Test]
        public void AcreditarCarga_SinPendiente_NoCambiaNada()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            tarjeta.AcreditarCarga();

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
        }

        [Test]
        public void AcreditarCarga_ConSaldoEnElMaximo_NoAcredita()
        {
            var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 4000 };

            tarjeta.AcreditarCarga();

            Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000));
        }

        [Test]
        public void AgregarSaldo_ConPendienteYSaldoNegativo_CubreLaDeudaYAcredita()
        {
            var tarjeta = new Tarjeta { Saldo = -580 };

            tarjeta.AgregarSaldo(5000);

            Assert.That(tarjeta.Saldo, Is.EqualTo(4420));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
        }
        [TestCase(1, 790)]
        [TestCase(2, 1580)]
        public void CalcularTarifa_ConViajesDelDia_AplicaElLimiteDelTipo(int viajesHoy, int esperado)
        {
            var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo { PorcentajeTarifa = 50, ViajesConBeneficioPorDia = 2 } };

            Assert.That(tarjeta.CalcularTarifa(viajesHoy), Is.EqualTo(esperado));
        }

        [Test]
        public void DescontarSaldo_MontoCero_NoCambiaElSaldo()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            tarjeta.DescontarSaldo(0);

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }
    }
}
