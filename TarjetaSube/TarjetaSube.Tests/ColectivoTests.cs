using System;
using System.Collections.Generic;
using System.Text;

namespace TarjetaSube.Tests
{
    public class ColectivoTests : BaseDeTests
    {

        [Test]
        public void Crear_LineaValida_GuardaConRecaudacionCero()
        {
            var colectivo = Colectivo.Crear("K");

            Contexto.Db.ChangeTracker.Clear();
            var guardado = Contexto.Db.Colectivos.Find(colectivo.Id);

            Assert.That(guardado, Is.Not.Null);
            Assert.That(guardado!.Linea, Is.EqualTo("K"));
            Assert.That(guardado.Recaudacion, Is.EqualTo(0));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void Crear_LineaVacia_LanzaExcepcion(string linea)
        {
            Assert.Throws<ArgumentException>(() => Colectivo.Crear(linea));
        }

        [Test]
        public void Modificar_LineaValida_ActualizaYPersiste()
        {
            var colectivo = Colectivo.Crear("K");

            colectivo.Modificar("101");

            Contexto.Db.ChangeTracker.Clear();
            Assert.That(Contexto.Db.Colectivos.Find(colectivo.Id)!.Linea, Is.EqualTo("101"));
        }

        [Test]
        public void Modificar_LineaVacia_LanzaExcepcionYNoCambia()
        {
            var colectivo = Colectivo.Crear("K");

            Assert.Throws<ArgumentException>(() => colectivo.Modificar(""));
            Assert.That(colectivo.Linea, Is.EqualTo("K"));
        }

        [Test]
        public void Eliminar_SinBoletos_BorraElColectivo()
        {
            var colectivo = Colectivo.Crear("K");

            colectivo.Eliminar();

            Assert.That(Contexto.Db.Colectivos.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Eliminar_ConBoletos_LanzaExcepcionYNoBorra()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            colectivo.PagarCon(tarjeta);

            Assert.Throws<InvalidOperationException>(() => colectivo.Eliminar());
            Assert.That(Contexto.Db.Colectivos.Count(), Is.EqualTo(1));
        }


        [Test]
        public void PagarCon_SaldoSuficiente_DescuentaLaTarifa()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta);

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_SumaLaRecaudacion()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(2 * Reglas.TarifaBasica));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_DevuelveBoletoConLosDatosCorrectos()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            var boleto = colectivo.PagarCon(tarjeta);

            Assert.That(boleto.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
            Assert.That(boleto.FechaHora, Is.EqualTo(FechaFija));
            Assert.That(boleto.TarjetaId, Is.EqualTo(tarjeta.Id));
            Assert.That(boleto.ColectivoId, Is.EqualTo(colectivo.Id));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_PersisteSaldoRecaudacionYBoleto()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta);

            Contexto.Db.ChangeTracker.Clear();
            Assert.That(Contexto.Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(3420));
            Assert.That(Contexto.Db.Colectivos.Find(colectivo.Id)!.Recaudacion, Is.EqualTo(1580));
            Assert.That(Contexto.Db.Boletos.Count(), Is.EqualTo(1));
        }

        [Test]
        public void PagarCon_SaldoJusto_PermiteElViajeYQuedaEnCero()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.Saldo = Reglas.TarifaBasica;

            colectivo.PagarCon(tarjeta);

            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_SaldoInsuficiente_LanzaExcepcionYNoDejaRastros()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");
            tarjeta.Saldo = 1000;

            Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta));

            Assert.That(tarjeta.Saldo, Is.EqualTo(1000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
            Assert.That(Contexto.Db.Boletos.Count(), Is.EqualTo(0));
        }


        [Test]
        public void CargarTarjeta_MontoValido_SumaSaldoYPersiste()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");

            colectivo.CargarTarjeta(tarjeta, 5000);

            Contexto.Db.ChangeTracker.Clear();
            Assert.That(Contexto.Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void CargarTarjeta_MontoInvalido_LanzaExcepcionYNoCambiaSaldo()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");

            Assert.Throws<ArgumentException>(() => colectivo.CargarTarjeta(tarjeta, 1000));
            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void CargarTarjeta_NoModificaLaRecaudacion()
        {
            var colectivo = Colectivo.Crear("K");
            var tarjeta = Tarjeta.Crear(1, "Ana");

            colectivo.CargarTarjeta(tarjeta, 5000);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }
    }
}
