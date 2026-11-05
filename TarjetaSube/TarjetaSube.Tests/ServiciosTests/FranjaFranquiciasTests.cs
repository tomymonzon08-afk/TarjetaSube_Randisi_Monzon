namespace TarjetaSube.Tests
{
    public class FranjaFranquiciasTests : BaseDeTests
    {
        private static readonly DateTime[] FueraDeLaFranja =
        {
            new(2026, 9, 26, 12, 0, 0), // sábado
            new(2026, 9, 27, 12, 0, 0), // domingo
            new(2026, 9, 21, 5, 59, 0), // lunes, un minuto antes
            new(2026, 9, 21, 22, 0, 0) // lunes, justo a las 22
        };

        private static readonly DateTime[] DentroDeLaFranja =
        {
            new(2026, 9, 21, 6, 0, 0), // lunes 6:00
            new(2026, 9, 21, 21, 59, 0), // lunes 21:59
            new(2026, 9, 25, 12, 0, 0) // viernes
        };

        [Test]
        public void Pagar_FranquiciaFueraDeLaFranja_SeRechazaYNoGuardaBoleto(
            [Values(2, 3, 4)] int tipoId, [ValueSource(nameof(FueraDeLaFranja))]
            DateTime fecha)
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            Reloj.Fecha = fecha;

            var resultado = Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(Db.Boletos.Count(), Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }

        [Test]
        public void Pagar_FranquiciaDentroDeLaFranja_Paga(
            [Values(2, 3, 4)] int tipoId, [ValueSource(nameof(DentroDeLaFranja))]
            DateTime fecha)
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            Reloj.Fecha = fecha;

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
            Assert.That(Db.Boletos.Count(), Is.EqualTo(1));
        }

        [Test]
        public void
        Pagar_TarjetaNormal_NoTieneFranja([ValueSource(nameof(FueraDeLaFranja))] DateTime
        fecha)
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            Reloj.Fecha = fecha;

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
        }
    }
}