using TarjetaSube.Tests;

namespace TarjetaSube
{
    public class CargaServicioTests : BaseDeTests
    {
        [Test]
        public void CargarTarjeta_MontoValido_SumaSaldoYPersiste()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Cargas.Cargar(tarjeta, 5000);

            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void CargarTarjeta_MontoInvalido_LanzaExcepcionYNoCambiaSaldo()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Assert.Throws<ArgumentException>(() => Cargas.Cargar(tarjeta, 1000));
            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void Cargar_SuperaElMaximo_PersisteSaldoYPendiente()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Cargas.Cargar(tarjeta, 30000);
            Cargas.Cargar(tarjeta, 30000);   

            Db.ChangeTracker.Clear();
            var guardada = Db.Tarjetas.Find(tarjeta.Id)!;
            Assert.That(guardada.Saldo, Is.EqualTo(56000));
            Assert.That(guardada.SaldoPendiente, Is.EqualTo(4000));
        }

    }
}