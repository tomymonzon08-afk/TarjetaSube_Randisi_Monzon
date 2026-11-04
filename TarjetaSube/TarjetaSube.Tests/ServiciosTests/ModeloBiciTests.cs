namespace TarjetaSube.Tests
{
    public class ModeloBiciTests : BaseDeTests
    {
        [Test]
        public void MultasBiciPendientes_SePersisteEnLaTarjeta()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.MultasBiciPendientes = 2;
            Db.SaveChanges();
            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.MultasBiciPendientes,
            Is.EqualTo(2));
        }

        [Test]
        public void AlquilerBici_SePersisteConSusDatosYRelaciones()
        {
            var estacion = Estaciones.Crear("Centro");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var alquiler = new AlquilerBici
            {
                FechaRetiro = FechaFija,
                TarifaCobrada = 2418.75m,
                MultasCobradas = 1,
                PagoElDia = true,
                Tarjeta = tarjeta,
                Estacion = estacion
            };
            Db.AlquileresBici.Add(alquiler);
            Db.SaveChanges();
            Db.ChangeTracker.Clear();
            var guardado = Db.AlquileresBici.Find(alquiler.Id)!;
            Assert.That(guardado.FechaRetiro, Is.EqualTo(FechaFija));
            Assert.That(guardado.FechaDevolucion, Is.Null);
            Assert.That(guardado.TarifaCobrada, Is.EqualTo(2418.75m));
            Assert.That(guardado.MultasCobradas, Is.EqualTo(1));
            Assert.That(guardado.PagoElDia, Is.True);
            Assert.That(guardado.TarjetaId, Is.EqualTo(tarjeta.Id));
            Assert.That(guardado.EstacionId, Is.EqualTo(estacion.Id));
        }
    }
}