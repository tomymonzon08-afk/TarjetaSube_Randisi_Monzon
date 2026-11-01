namespace TarjetaSube.Tests
{
    public class AcreditacionTests : BaseDeTests
    {
        [Test]
        public void Pagar_ConSaldoPendiente_AcreditaHastaVolverAlMaximo()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = 56000;
            tarjeta.SaldoPendiente = 4000;

            Viajes.Pagar(colectivo, tarjeta, out _);

            Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000 - Reglas.TarifaBasica));
        }

        [Test]
        public void Pagar_VariosViajes_VaVaciandoElPendienteSinPasarseDelMaximo()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = 56000;
            tarjeta.SaldoPendiente = 4000;

            for (int i = 0; i < 3; i++)
                Viajes.Pagar(colectivo, tarjeta, out _);

            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(56000 - 3 * Reglas.TarifaBasica + 4000));
        }

        [Test]
        public void Pagar_ConPendiente_PersisteSaldoYPendiente()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.Saldo = 56000;
            tarjeta.SaldoPendiente = 4000;

            Viajes.Pagar(colectivo, tarjeta, out _);

            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.SaldoPendiente, Is.EqualTo(2420));
        }
    }
}