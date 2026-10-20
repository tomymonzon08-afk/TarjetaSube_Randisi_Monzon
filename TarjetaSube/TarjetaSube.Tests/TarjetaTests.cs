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
            var tarjeta = new Tarjeta { Saldo = 1000 };

            Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(1580));
            Assert.That(tarjeta.Saldo, Is.EqualTo(1000));
        }

        [TestCase(0)]
        [TestCase(-100)]
        public void DescontarSaldo_MontoNoPositivo_LanzaExcepcion(int monto)
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };

            Assert.Throws<ArgumentException>(() => tarjeta.DescontarSaldo(monto));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void Crear_DatosValidos_GuardaLaTarjetaConSaldoCero()
        {
            var tarjeta = Tarjeta.Crear(1234567890123456, "Ana");

            Contexto.Db.ChangeTracker.Clear();
            var guardada = Contexto.Db.Tarjetas.Find(tarjeta.Id);

            Assert.That(guardada, Is.Not.Null);
            Assert.That(guardada!.Numero, Is.EqualTo(1234567890123456));
            Assert.That(guardada.Dueno, Is.EqualTo("Ana"));
            Assert.That(guardada.Saldo, Is.EqualTo(0));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void Crear_NumeroInvalido_LanzaExcepcion(long numero)
        {
            Assert.Throws<ArgumentException>(() => Tarjeta.Crear(numero, "Ana"));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void Crear_DuenoVacio_LanzaExcepcion(string dueno)
        {
            Assert.Throws<ArgumentException>(() => Tarjeta.Crear(1, dueno));
        }

        [Test]
        public void Crear_NumeroRepetido_LanzaExcepcion()
        {
            Tarjeta.Crear(1, "Ana");

            Assert.Throws<InvalidOperationException>(() => Tarjeta.Crear(1, "Beto"));
        }

        [Test]
        public void Modificar_DuenoValido_ActualizaYPersiste()
        {
            var tarjeta = Tarjeta.Crear(1, "Ana");

            tarjeta.Modificar("Beto");

            Contexto.Db.ChangeTracker.Clear();
            Assert.That(Contexto.Db.Tarjetas.Find(tarjeta.Id)!.Dueno, Is.EqualTo("Beto"));
        }

        [Test]
        public void Modificar_DuenoVacio_LanzaExcepcionYNoCambia()
        {
            var tarjeta = Tarjeta.Crear(1, "Ana");

            Assert.Throws<ArgumentException>(() => tarjeta.Modificar(" "));
            Assert.That(tarjeta.Dueno, Is.EqualTo("Ana"));
        }

        [Test]
        public void Eliminar_SinBoletos_BorraLaTarjeta()
        {
            var tarjeta = Tarjeta.Crear(1, "Ana");

            tarjeta.Eliminar();

            Assert.That(Contexto.Db.Tarjetas.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Eliminar_ConBoletos_LanzaExcepcionYNoBorra()
        {
            var tarjeta = Tarjeta.Crear(1, "Ana");
            var colectivo = Colectivo.Crear("K");
            tarjeta.AgregarSaldo(5000);
            colectivo.PagarCon(tarjeta);

            Assert.Throws<InvalidOperationException>(() => tarjeta.Eliminar());
            Assert.That(Contexto.Db.Tarjetas.Count(), Is.EqualTo(1));
        }
    }
}
