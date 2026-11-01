namespace TarjetaSube.Tests
{
    public class LimiteMedioBoletoTests : BaseDeTests
    {
        private Tarjeta MedioBoletoConSaldo()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);
            tarjeta.AgregarSaldo(5000);
            return tarjeta;
        }

        [Test]
        public void SegundoViajeAntesDeCincoMinutos_SeRechaza()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = MedioBoletoConSaldo();
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(4));

            var resultado = Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(Db.Boletos.Count(), Is.EqualTo(1));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 790));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(790));
        }

        [Test]
        public void SegundoViajeAPartirDeCincoMinutos_SePermite()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = MedioBoletoConSaldo();
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(5));

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
        }

        [Test]
        public void TercerViajeDelDia_SeCobraTarifaNormal()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = MedioBoletoConSaldo();
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(5));
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(5));

            Viajes.Pagar(colectivo, tarjeta, out var tercero);

            Assert.That(tercero!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
        }

        [Test]
        public void NoSePermitenMasDeDosViajesAMitadDePrecioPorDia()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = MedioBoletoConSaldo();

            for (int i = 0; i < 4; i++)
            {
                Viajes.Pagar(colectivo, tarjeta, out _);
                Reloj.Avanzar(TimeSpan.FromMinutes(5));
            }

            Assert.That(Db.Boletos.Count(b => b.Tarifa == 790), Is.EqualTo(2));
            Assert.That(Db.Boletos.Count(b => b.Tarifa == Reglas.TarifaBasica), Is.EqualTo(2));
        }

        [Test]
        public void AlCambiarElDia_VuelveElMedioBoleto()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = MedioBoletoConSaldo();
            Viajes.Pagar(colectivo, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(5));
            Viajes.Pagar(colectivo, tarjeta, out _);

            Reloj.Fecha = FechaFija.AddDays(1);
            Viajes.Pagar(colectivo, tarjeta, out var boleto);

            Assert.That(boleto!.Tarifa, Is.EqualTo(790));
        }

        [Test]
        public void TarjetaNormal_NoTieneLimiteDeTiempoNiDeViajes()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(8000);

            Assert.That(Viajes.Pagar(colectivo, tarjeta, out _), Is.True);
            Assert.That(Viajes.Pagar(colectivo, tarjeta, out var segundo), Is.True);
            Assert.That(segundo!.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
        }
    }
}
