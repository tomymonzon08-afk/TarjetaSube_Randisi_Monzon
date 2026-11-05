namespace TarjetaSube.Tests
{
    public class RetiroBiciServicioTests : BaseDeTests
    {
        private Tarjeta TarjetaConSaldo(int tipoId = 1)
        {
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            return tarjeta;
        }

        [Test]
        public void Retirar_SaldoSuficiente_PersisteAlquilerSaldoYRecaudacion()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo();

            Retiros.Retirar(estacion, tarjeta, out var alquiler);

            Db.ChangeTracker.Clear();
            var guardado = Db.AlquileresBici.Find(alquiler!.Id)!;
            Assert.That(guardado.TarifaCobrada, Is.EqualTo(Reglas.TarifaBici));
            Assert.That(guardado.FechaRetiro, Is.EqualTo(FechaFija));
            Assert.That(guardado.TarjetaId, Is.EqualTo(tarjeta.Id));
            Assert.That(guardado.EstacionId, Is.EqualTo(estacion.Id));
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(5000 - Reglas.TarifaBici));
            Assert.That(Db.EstacionesBici.Find(estacion.Id)!.Recaudacion, Is.EqualTo(Reglas.TarifaBici));
        }

        [Test]
        public void Retirar_SinSaldo_SeRechazaYNoGuardaNada()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");   // saldo 0

            Assert.That(Retiros.Retirar(estacion, tarjeta, out var alquiler), Is.False);
            Assert.That(alquiler, Is.Null);
            Assert.That(Db.AlquileresBici.Count(), Is.EqualTo(0));
            Assert.That(estacion.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void Retirar_ConBiciEnUso_SeRechazaElSegundoRetiro()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo();
            Retiros.Retirar(estacion, tarjeta, out _);

            Assert.That(Retiros.Retirar(estacion, tarjeta, out _), Is.False);
            Assert.That(Db.AlquileresBici.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Retirar_SegundaVezElMismoDia_NoCobra()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo();
            Retiros.Retirar(estacion, tarjeta, out var primero);
            primero!.FechaDevolucion = Reloj.Ahora();   // devuelta a mano
            Db.SaveChanges();
            Reloj.Avanzar(TimeSpan.FromHours(2));

            var resultado = Retiros.Retirar(estacion, tarjeta, out var segundo);

            Assert.That(resultado, Is.True);
            Assert.That(segundo!.TarifaCobrada, Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBici));
        }

        [Test]
        public void Retirar_AlDiaSiguiente_VuelveACobrar()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo();
            Retiros.Retirar(estacion, tarjeta, out var primero);
            primero!.FechaDevolucion = Reloj.Ahora();
            Db.SaveChanges();
            Reloj.Fecha = FechaFija.AddDays(1);

            Retiros.Retirar(estacion, tarjeta, out var segundo);

            Assert.That(segundo!.TarifaCobrada, Is.EqualTo(Reglas.TarifaBici));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 2 * Reglas.TarifaBici));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Retirar_CualquierTarjeta_PagaLoMismoSinFranja(int tipoId)
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo(tipoId);
            Reloj.Fecha = new DateTime(2026, 9, 27, 23, 0, 0);   // domingo de noche

            Assert.That(Retiros.Retirar(estacion, tarjeta, out var alquiler), Is.True);
            Assert.That(alquiler!.TarifaCobrada, Is.EqualTo(Reglas.TarifaBici));
        }

        [Test]
        public void ObtenerHistorial_SinAlquileres_DevuelveVacio()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Assert.That(Retiros.ObtenerHistorial(tarjeta, Reloj.Ahora()), Is.EqualTo(HistorialBici.Vacio));
        }

        [Test]
        public void ObtenerHistorial_ConUnRetiroSinDevolver_MarcaBiciEnUsoYPagoHoy()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo();
            Retiros.Retirar(estacion, tarjeta, out _);

            Assert.That(Retiros.ObtenerHistorial(tarjeta, Reloj.Ahora()), Is.EqualTo(new HistorialBici(true, true)));
        }
    }
}
