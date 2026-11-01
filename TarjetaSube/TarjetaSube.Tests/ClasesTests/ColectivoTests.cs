using System;
using System.Collections.Generic;
using System.Text;

namespace TarjetaSube.Tests
{
    public class ColectivoTests : BaseDeTests
    {
        [Test]
        public void PagarCon_SaldoSuficiente_DescuentaLaTarifa()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta, out _);

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_SumaLaRecaudacion()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta, out _);
            colectivo.PagarCon(tarjeta, out _);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(2 * Reglas.TarifaBasica));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_DevuelveBoletoConLosDatosCorrectos()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta, out var boleto);

            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
            Assert.That(boleto.FechaHora, Is.EqualTo(FechaFija));
            Assert.That(boleto.TarjetaId, Is.EqualTo(tarjeta.Id));
            Assert.That(boleto.ColectivoId, Is.EqualTo(colectivo.Id));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_PersisteSaldoRecaudacionYBoleto()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta, out var boleto); 

            Contexto.Db.ChangeTracker.Clear();
            Assert.That(Contexto.Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(3420));
            Assert.That(Contexto.Db.Colectivos.Find(colectivo.Id)!.Recaudacion, Is.EqualTo(1580));
            Assert.That(Contexto.Db.Boletos.Count(), Is.EqualTo(1));
        }

        [Test]
        public void PagarCon_SaldoJusto_PermiteElViajeYQuedaEnCero()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = Reglas.TarifaBasica;

            colectivo.PagarCon(tarjeta, out var boleto); 

            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_SaldoInsuficiente_DevuelveFalseYNoDejaRastros()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = -1000;

            var resultado = colectivo.PagarCon(tarjeta, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
            Assert.That(Contexto.Db.Boletos.Count(), Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_DevuelveTrueYBoleto()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            var resultado = colectivo.PagarCon(tarjeta, out var boleto);

            Assert.That(resultado, Is.True);
            Assert.That(boleto, Is.Not.Null);
        }



        [Test]
        public void CargarTarjeta_MontoValido_SumaSaldoYPersiste()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");

            colectivo.CargarTarjeta(tarjeta, 5000);

            Contexto.Db.ChangeTracker.Clear();
            Assert.That(Contexto.Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void CargarTarjeta_MontoInvalido_LanzaExcepcionYNoCambiaSaldo()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Assert.Throws<ArgumentException>(() => colectivo.CargarTarjeta(tarjeta, 1000));
            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void CargarTarjeta_NoModificaLaRecaudacion()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");

            colectivo.CargarTarjeta(tarjeta, 5000);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_MedioBoleto_ElMontoEsLaMitadDelNormal()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica / 2));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica / 2));
        }

        [Test]
        public void PagarCon_FranquiciaCompleta_SiemprePuedePagar()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 4); 

            for (int i = 0; i < 5; i++)
            {
                Assert.That(colectivo.PagarCon(tarjeta, out var boleto), Is.True);
                Assert.That(boleto!.Tarifa, Is.EqualTo(0));
            }
            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_MedioBoleto_LaRecaudacionSumaLaTarifaReal()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);
            tarjeta.AgregarSaldo(5000);

            colectivo.PagarCon(tarjeta, out _);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(790));
        }

        [Test]
        public void PagarCon_FranquiciaCompleta_ConSaldoMinimo_TambienPuedePagar()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 4); 
            tarjeta.Saldo = Reglas.SaldoMinimo; 

            var resultado = colectivo.PagarCon(tarjeta, out var boleto);

            Assert.That(resultado, Is.True);
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto!.Tarifa, Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(Reglas.SaldoMinimo));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }
        [Test]
        public void PagarCon_NoPermiteQuedarPorDebajoDelSaldoMinimo()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = -500; 

            var resultado = colectivo.PagarCon(tarjeta, out _);

            Assert.That(resultado, Is.False);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-500));
        }

        [Test]
        public void PagarCon_ViajesPlus_DescuentaCorrectamenteYLuegoLaCargaSaldaLaDeuda()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = 1000;

            Assert.That(colectivo.PagarCon(tarjeta, out _), Is.True);  
            Assert.That(tarjeta.Saldo, Is.EqualTo(-580));
            Assert.That(colectivo.PagarCon(tarjeta, out _), Is.False);  
            Assert.That(tarjeta.Saldo, Is.EqualTo(-580));

            colectivo.CargarTarjeta(tarjeta, 5000);                    
            Assert.That(tarjeta.Saldo, Is.EqualTo(4420));
        }

        [Test]
        public void PagarCon_LlegaExactoAlSaldoMinimo_Permite()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = -420;

            Assert.That(colectivo.PagarCon(tarjeta, out _), Is.True);
            Assert.That(tarjeta.Saldo, Is.EqualTo(Reglas.SaldoMinimo));
        }

    }
}
