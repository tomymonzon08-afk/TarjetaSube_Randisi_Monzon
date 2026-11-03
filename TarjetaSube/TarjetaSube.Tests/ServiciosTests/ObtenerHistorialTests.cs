namespace TarjetaSube.Tests
{
    public class ObtenerHistorialTests : BaseDeTests
    {
        [Test]
        public void CuentaLosViajesDelMesCalendario()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(8000);

            Reloj.Fecha = new DateTime(2026, 8, 31, 10, 0, 0);   // mes anterior
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Fecha = new DateTime(2026, 9, 1, 8, 0, 0);
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Fecha = FechaFija;
            Viajes.Pagar(colectivo, tarjeta, out _);

            var historial = Viajes.ObtenerHistorial(tarjeta, FechaFija);

            Assert.That(historial.ViajesDelMes, Is.EqualTo(2));
            Assert.That(historial.ViajesHoy, Is.EqualTo(1));
        }

        [Test]
        public void DevuelveLaLineaDelUltimoBoleto()
        {
            var k = Colectivos.Crear("K");
            var c101 = Colectivos.Crear("101");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(8000);
            Viajes.Pagar(k, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(10));
            Viajes.Pagar(c101, tarjeta, out _);

            Assert.That(Viajes.ObtenerHistorial(tarjeta, Reloj.Ahora()).LineaUltimoBoleto, Is.EqualTo("101"));
        }

        [Test]
        public void IgnoraLosTrasbordosEnLosConteosPeroNoEnLaLinea()
        {
            var k = Colectivos.Crear("K");
            var c101 = Colectivos.Crear("101");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            Viajes.Pagar(k, tarjeta, out _);
            Db.Boletos.Add(new Boleto
            {
                FechaHora = FechaFija.AddMinutes(20), Tarifa = 0, Tarjeta = tarjeta, Colectivo = c101,
                TipoTarjeta = "Normal", Linea = "101", EsTransbordo = true
            });
            Db.SaveChanges();

            var historial = Viajes.ObtenerHistorial(tarjeta, FechaFija.AddMinutes(30));

            Assert.That(historial.ViajesHoy, Is.EqualTo(1));
            Assert.That(historial.ViajesDelMes, Is.EqualTo(1));
            Assert.That(historial.UltimoViaje, Is.EqualTo(FechaFija));
            Assert.That(historial.LineaUltimoBoleto, Is.EqualTo("101"));
        }
    }
}
