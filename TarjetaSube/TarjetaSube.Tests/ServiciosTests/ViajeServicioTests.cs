namespace TarjetaSube.Tests{
    public class ViajeServicioTests : BaseDeTests
    {
        [Test]
        public void PagarCon_SaldoSuficiente_DescuentaLaTarifa()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo,tarjeta, out _);

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_SumaLaRecaudacion()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo,tarjeta, out _);
            Viajes.Pagar(colectivo,tarjeta, out _);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(2 * Reglas.TarifaBasica));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_DevuelveBoletoConLosDatosCorrectos()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo,tarjeta, out var boleto);

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

            Viajes.Pagar(colectivo, tarjeta, out var boleto); 

            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(3420));
            Assert.That(Db.Colectivos.Find(colectivo.Id)!.Recaudacion, Is.EqualTo(1580));
            Assert.That(Db.Boletos.Count(), Is.EqualTo(1));
        }

        [Test]
        public void PagarCon_SaldoJusto_PermiteElViajeYQuedaEnCero()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = Reglas.TarifaBasica;

            Viajes.Pagar(colectivo, tarjeta, out var boleto); 

            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_SaldoInsuficiente_DevuelveFalseYNoDejaRastros()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = -1000;

            var resultado = Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
            Assert.That(Db.Boletos.Count(), Is.EqualTo(0));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_DevuelveTrueYBoleto()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            var resultado = Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(resultado, Is.True);
            Assert.That(boleto, Is.Not.Null);
        }
        
        [Test]
        public void PagarCon_MedioBoleto_ElMontoEsLaMitadDelNormal()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

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
                Assert.That(Viajes.Pagar(colectivo, tarjeta, out var boleto), Is.True);
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

            Viajes.Pagar(colectivo, tarjeta, out _);

            Assert.That(colectivo.Recaudacion, Is.EqualTo(790));
        }

        [Test]
        public void PagarCon_FranquiciaCompleta_ConSaldoMinimo_TambienPuedePagar()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 4); 
            tarjeta.Saldo = Reglas.SaldoMinimo; 

            var resultado = Viajes.Pagar(colectivo, tarjeta, out var boleto);

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

            var resultado = Viajes.Pagar(colectivo, tarjeta, out _);

            Assert.That(resultado, Is.False);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-500));
        }

        [Test]
        public void PagarCon_ViajesPlus_DescuentaCorrectamenteYLuegoLaCargaSaldaLaDeuda()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = 1000;

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);  
            Assert.That(tarjeta.Saldo, Is.EqualTo(-580));
            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.False);  
            Assert.That(tarjeta.Saldo, Is.EqualTo(-580));

            Cargas.Cargar(tarjeta, 5000);                    
            Assert.That(tarjeta.Saldo, Is.EqualTo(4420));
        }

        [Test]
        public void PagarCon_LlegaExactoAlSaldoMinimo_Permite()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = -420;

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
            Assert.That(tarjeta.Saldo, Is.EqualTo(Reglas.SaldoMinimo));
        }

        [Test]
        public void Pagar_GuardaElBoletoConFechaTarifaYRelaciones()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var colectivo = Colectivos.Crear("K");
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Db.ChangeTracker.Clear();
            var guardado = Db.Boletos.Find(boleto!.Id);

            Assert.That(guardado, Is.Not.Null);
            Assert.That(guardado!.FechaHora, Is.EqualTo(FechaFija));
            Assert.That(guardado.Tarifa, Is.EqualTo(1580));
            Assert.That(guardado.TarjetaId, Is.EqualTo(tarjeta.Id));
            Assert.That(guardado.ColectivoId, Is.EqualTo(colectivo.Id));
        }

        [Test]
        public void Pagar_UsaLaFechaDelProveedor()
        {
            var otraFecha = new DateTime(2027, 1, 15, 8, 30, 0);
            Reloj.Fecha = otraFecha;
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var colectivo = Colectivos.Crear("K");
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.FechaHora, Is.EqualTo(otraFecha));
        }

        [Test]
        public void Pagar_DosViajesConElRelojAvanzado_GuardaCadaFecha()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var colectivo = Colectivos.Crear("K");
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo, tarjeta, out var primero);
            Reloj.Avanzar(TimeSpan.FromMinutes(6));
            Viajes.Pagar(colectivo, tarjeta, out var segundo);

            Assert.That(primero!.FechaHora, Is.EqualTo(FechaFija));
            Assert.That(segundo!.FechaHora, Is.EqualTo(FechaFija.AddMinutes(6)));
        }

        [Test]
        public void ObtenerHistorial_SinViajes_DevuelveVacio()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Assert.That(Viajes.ObtenerHistorial(tarjeta, Reloj.Ahora()), Is.EqualTo(HistorialDeViajes.Vacio));
        }

        [Test]
        public void ObtenerHistorial_ContaSoloLosViajesDelDiaYDevuelveElUltimo()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var colectivo = Colectivos.Crear("K");
            tarjeta.AgregarSaldo(8000);

            Reloj.Fecha = FechaFija.AddDays(-1);
            Viajes.Pagar(colectivo, tarjeta, out _);       
            Reloj.Fecha = FechaFija;
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(7));
            Viajes.Pagar(colectivo, tarjeta, out _);

            var historial = Viajes.ObtenerHistorial(tarjeta, Reloj.Ahora());

            Assert.That(historial.ViajesHoy, Is.EqualTo(2));
            Assert.That(historial.UltimoViaje, Is.EqualTo(FechaFija.AddMinutes(7)));
        }

        [Test]
        public void Pagar_GuardaTipoLineaSaldoRestanteEIdDeLaTarjeta()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);
            tarjeta.AgregarSaldo(5000);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Db.ChangeTracker.Clear();
            var guardado = Db.Boletos.Find(boleto!.Id)!;
            Assert.That(guardado.TipoTarjeta, Is.EqualTo("Medio boleto estudiantil"));
            Assert.That(guardado.Linea, Is.EqualTo("K"));
            Assert.That(guardado.SaldoRestante, Is.EqualTo(4210));
            Assert.That(guardado.TarjetaId, Is.EqualTo(tarjeta.Id));
        }

    }
}