namespace TarjetaSube.Tests
{
    public class UsoFrecuenteServicioTests : BaseDeTests
    {
        private static readonly DateTime InicioDeMes = new(2026, 9, 1, 8, 0, 0);

        private void AgregarViajesPrevios(Tarjeta tarjeta, Colectivo colectivo, int cantidad, DateTime primero)
        {
            for (int i = 0; i < cantidad; i++)
                Db.Boletos.Add(new Boleto
                {
                    FechaHora = primero.AddMinutes(i), Tarifa = Reglas.TarifaBasica,
                    Tarjeta = tarjeta, Colectivo = colectivo, TipoTarjeta = "Normal", Linea = colectivo.Linea
                });
            Db.SaveChanges();
        }

        [TestCase(28, 100)]
        [TestCase(29, 80)]
        [TestCase(78, 80)]
        [TestCase(79, 75)]
        public void Pagar_SegunLosViajesPreviosDelMes_CobraElTramoCorrespondiente(int previos, int porcentajePagado)
        {
            var esperado = Reglas.TarifaBasica * porcentajePagado / 100m;

            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            AgregarViajesPrevios(tarjeta, colectivo, previos, InicioDeMes);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(esperado));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - esperado));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(esperado));
        }

        [Test]
        public void Pagar_LosViajesDelMesAnteriorNoCuentan()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            AgregarViajesPrevios(tarjeta, colectivo, 40, new DateTime(2026, 8, 31, 20, 0, 0));

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
        }

        [Test]
        public void Pagar_ElContadorSeReiniciaAlComenzarOtroMes()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            AgregarViajesPrevios(tarjeta, colectivo, 40, InicioDeMes);
            Reloj.Fecha = new DateTime(2026, 10, 1, 8, 0, 0);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
        }

        [Test]
        public void Pagar_MedioBoleto_NoRecibeElDescuentoDeUsoFrecuente()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);
            tarjeta.AgregarSaldo(5000);
            AgregarViajesPrevios(tarjeta, colectivo, 40, InicioDeMes);

            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica / 2));
        }
    }
}
