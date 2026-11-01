namespace TarjetaSube.Tests
{
    public class LimiteBoletoGratuitoTests : BaseDeTests
    {
        private Tarjeta TarjetaConSaldo(int tipoId)
        {
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            return tarjeta;
        }

        [Test]
        public void LosDosPrimerosViajesDelDia_SonGratisYNoCambianElSaldo()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(3);

            Viajes.Pagar(colectivo, tarjeta, out var primero);
            Viajes.Pagar(colectivo, tarjeta, out var segundo);

            Assert.That(primero!.Tarifa, Is.EqualTo(0));
            Assert.That(segundo!.Tarifa, Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void ElTercerViajeYLosSiguientes_SeCobranATarifaCompleta()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(3);
            Viajes.Pagar(colectivo, tarjeta, out _);
            Viajes.Pagar(colectivo, tarjeta, out _);

            Viajes.Pagar(colectivo, tarjeta, out var tercero);
            Viajes.Pagar(colectivo, tarjeta, out var cuarto);

            Assert.That(tercero!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
            Assert.That(cuarto!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 2 * Reglas.TarifaBasica));
        }

        [Test]
        public void EnCuatroViajes_HayExactamenteDosGratuitos()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(3);

            for (int i = 0; i < 4; i++)
                Viajes.Pagar(colectivo, tarjeta, out _);

            Assert.That(Db.Boletos.Count(b => b.Tarifa == 0), Is.EqualTo(2));
            Assert.That(Db.Boletos.Count(b => b.Tarifa == Reglas.TarifaBasica), Is.EqualTo(2));
        }

        [Test]
        public void NoHayLimiteDeMinutos_DosViajesEnElMismoMinutoSeAceptan()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(3);

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
        }

        [Test]
        public void AlCambiarElDia_VuelvenLosDosViajesGratis()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(3);
            for (int i = 0; i < 3; i++)
                Viajes.Pagar(colectivo, tarjeta, out _);   // el tercero ya se cobra

            Reloj.Fecha = FechaFija.AddDays(1);
            Viajes.Pagar(colectivo, tarjeta, out var primeroDelDiaSiguiente);
            Viajes.Pagar(colectivo, tarjeta, out var segundoDelDiaSiguiente);

            Assert.That(primeroDelDiaSiguiente!.Tarifa, Is.EqualTo(0));
            Assert.That(segundoDelDiaSiguiente!.Tarifa, Is.EqualTo(0));
        }

        [Test]
        public void FranquiciaCompleta_SigueSinLimite()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = TarjetaConSaldo(4);

            for (int i = 0; i < 5; i++)
            {
                Assert.That(Viajes.Pagar(colectivo, tarjeta, out var boleto), Is.True);
                Assert.That(boleto!.Tarifa, Is.EqualTo(0));
            }

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }
    }
}