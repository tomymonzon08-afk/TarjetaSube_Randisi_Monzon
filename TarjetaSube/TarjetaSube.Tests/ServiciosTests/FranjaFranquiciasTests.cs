namespace TarjetaSube.Tests
{
    public class FranjaFranquiciasTests : BaseDeTests
    {
        private Tarjeta TarjetaConSaldo(int tipoId)
        {
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            return tarjeta;
        }

        [TestCase(2, 2026, 9, 26, 10, 0)]    // sábado
        [TestCase(3, 2026, 9, 27, 10, 0)]    // domingo
        [TestCase(4, 2026, 9, 21, 5, 59)]    // lunes, un minuto antes de las 6
        [TestCase(2, 2026, 9, 21, 22, 0)]    // lunes a las 22:00
        [TestCase(3, 2026, 9, 25, 23, 30)]   // viernes de noche
        public void FueraDeLaFranja_NoSePuedePagarConFranquicias(int tipoId, int anio, int mes, int dia, int hora, int minuto)
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(tipoId);
            Reloj.Fecha = new DateTime(anio, mes, dia, hora, minuto, 0);

            var resultado = Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(Db.Boletos.Count(), Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }

        [TestCase(2, 2026, 9, 21, 6, 0)]     // lunes 6:00
        [TestCase(3, 2026, 9, 21, 21, 59)]   // lunes 21:59
        [TestCase(4, 2026, 9, 25, 12, 0)]    // viernes
        public void DentroDeLaFranja_SePuedePagarConFranquicias(int tipoId, int anio, int mes, int dia, int hora, int minuto)
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(tipoId);
            Reloj.Fecha = new DateTime(anio, mes, dia, hora, minuto, 0);

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
        }

        [TestCase(2026, 9, 26, 10, 0)]    // sábado
        [TestCase(2026, 9, 27, 23, 30)]   // domingo de noche
        [TestCase(2026, 9, 21, 3, 0)]     // lunes de madrugada
        public void TarjetaNormal_PuedeViajarFueraDeLaFranja(int anio, int mes, int dia, int hora, int minuto)
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(1);
            Reloj.Fecha = new DateTime(anio, mes, dia, hora, minuto, 0);

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
        }

        [Test]
        public void UnViajeRechazadoPorLaFranja_NoConsumeViajesDelDia()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(2);
            Reloj.Fecha = new DateTime(2026, 9, 26, 10, 0, 0);   // sábado
            Viajes.Pagar(colectivo, tarjeta, out _);

            Reloj.Fecha = FechaFija;                             // lunes
            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica / 2));
        }
    }
}
