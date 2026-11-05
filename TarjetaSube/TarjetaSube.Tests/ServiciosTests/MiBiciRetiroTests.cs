namespace TarjetaSube.Tests
{
    public class MiBiciRetiroTests : BaseDeTests
    {
        [Test]
        public void SePagaLaTarifaSeRetiraYLaDevolucionSinDemoraNoGeneraMulta()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);

            var retiro = Retiros.Retirar(estacion, tarjeta, out var alquiler);
            Reloj.Avanzar(TimeSpan.FromMinutes(45));
            var devolucion = Devoluciones.Devolver(alquiler!);

            Assert.That(retiro, Is.True);
            Assert.That(alquiler!.TarifaCobrada, Is.EqualTo(Reglas.TarifaBici));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBici));
            Assert.That(devolucion, Is.True);
            Assert.That(alquiler.ExcedioElTiempo, Is.False);
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
        }

        [Test]
        public void SinSaldoSuficienteYSinMulta_NoSePuedeRetirar()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(2000);   // menos que 2418,75

            Assert.That(Retiros.Retirar(estacion, tarjeta, out var alquiler), Is.False);
            Assert.That(alquiler, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(2000));
            Assert.That(Db.AlquileresBici.Count(), Is.EqualTo(0));
        }

        [TestCase(1000)]   // no alcanza ni para la tarifa
        [TestCase(3000)]   // alcanza para la tarifa pero no para tarifa + multa (3668,75)
        public void ConMultaPendienteYSinSaldoSuficiente_NoSePuedeRetirar(int saldo)
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = saldo;
            tarjeta.MultasBiciPendientes = 1;
            Db.SaveChanges();

            Assert.That(Retiros.Retirar(estacion, tarjeta, out _), Is.False);
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldo));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1));
            Assert.That(Db.AlquileresBici.Count(), Is.EqualTo(0));
        }
    }
}
