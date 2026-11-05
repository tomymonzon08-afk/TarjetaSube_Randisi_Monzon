namespace TarjetaSube.Tests
{
    public class MiBiciMultasTests : BaseDeTests
    {
        private Tarjeta TarjetaConSaldo(int carga)
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(carga);
            return tarjeta;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void RetirarConMultasAcumuladas_CobraLaTarifaMasCadaMulta(int multas)
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = 20000;
            tarjeta.MultasBiciPendientes = multas;
            Db.SaveChanges();
            decimal total = Reglas.TarifaBici + multas * Reglas.MultaBici;
            var resultado = Retiros.Retirar(estacion, tarjeta, out var alquiler);
            Assert.That(resultado, Is.True);
            Assert.That(alquiler!.MultasCobradas, Is.EqualTo(multas));
            Assert.That(alquiler.TotalAbonado, Is.EqualTo(total));
            Assert.That(tarjeta.Saldo, Is.EqualTo(20000 - total));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
            Assert.That(estacion.Recaudacion, Is.EqualTo(total));
        }

        [Test]
        public void UnaDevolucionTardia_GeneraLaMultaYSeCobraAlDiaSiguiente()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo(10000);
            Retiros.Retirar(estacion, tarjeta, out var primero);
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(primero!);
            Reloj.Fecha = FechaFija.AddDays(1);

            var resultado = Retiros.Retirar(estacion, tarjeta, out var segundo);
            decimal total = Reglas.TarifaBici + Reglas.MultaBici;
            Assert.That(primero!.ExcedioElTiempo, Is.True);
            Assert.That(resultado, Is.True);
            Assert.That(segundo!.TarifaCobrada, Is.EqualTo(Reglas.TarifaBici));
            Assert.That(segundo.MultasCobradas, Is.EqualTo(1));
            Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - Reglas.TarifaBici -
            total));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
            Assert.That(estacion.Recaudacion, Is.EqualTo(Reglas.TarifaBici + total));
        }

        [Test]
        public void VariasMultasDelMismoDia_SeCobranJuntasAlDiaSiguiente()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo(10000);
            Retiros.Retirar(estacion, tarjeta, out var primero);
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(primero!);
            Retiros.Retirar(estacion, tarjeta, out var segundo);
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(segundo!);
            Reloj.Fecha = FechaFija.AddDays(1);
            Retiros.Retirar(estacion, tarjeta, out var tercero);
            decimal total = Reglas.TarifaBici + 2 * Reglas.MultaBici;
            Assert.That(segundo!.TarifaCobrada, Is.EqualTo(0));
            Assert.That(tercero!.MultasCobradas, Is.EqualTo(2));
            Assert.That(tercero.TotalAbonado, Is.EqualTo(total));
            Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - Reglas.TarifaBici -
            total));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
        }

        [Test]
        public void LaMultaNoSeCobraElMismoDiaQueSeGenera()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = TarjetaConSaldo(10000);
            Retiros.Retirar(estacion, tarjeta, out var primero);
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(primero!);
            var resultado = Retiros.Retirar(estacion, tarjeta, out var segundo);
            Assert.That(resultado, Is.True);
            Assert.That(segundo!.MultasCobradas, Is.EqualTo(0));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1));
            Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - Reglas.TarifaBici));
        }

        [Test]
        public void ConUnaMultaYSaldoJusto_PuedeRetirarYQuedaEnCero()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = Reglas.TarifaBici + Reglas.MultaBici; 
            tarjeta.MultasBiciPendientes = 1;
            Db.SaveChanges();
            var resultado = Retiros.Retirar(estacion, tarjeta, out _);
            Assert.That(resultado, Is.True);
            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
        }
    }
}