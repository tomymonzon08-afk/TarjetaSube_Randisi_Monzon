namespace TarjetaSube.Tests
{
    public class TrasbordosTests : BaseDeTests
    {
        private Colectivo _k = null!;
        private Colectivo _c101 = null!;

        [SetUp]
        public void CrearLineas()
        {
            _k = Colectivos.Crear("K");
            _c101 = Colectivos.Crear("101");
        }

        private Tarjeta TarjetaConSaldo(int tipoId = 1)
        {
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            return tarjeta;
        }

        [Test]
        public void OtraLineaDentroDeLaHora_EsTrasbordoGratisYSeIndicaEnElBoleto()
        {
            var tarjeta = TarjetaConSaldo();
            Viajes.Pagar(_k, tarjeta, out var primero);
            Reloj.Avanzar(TimeSpan.FromMinutes(30));

            var resultado = Viajes.Pagar(_c101, tarjeta, out var segundo);

            Assert.That(resultado, Is.True);
            Assert.That(primero!.EsTransbordo, Is.False);
            Assert.That(segundo!.EsTransbordo, Is.True);
            Assert.That(segundo.Tarifa, Is.EqualTo(0));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica));
            Assert.That(_c101.Recaudacion, Is.EqualTo(0));
        }

        [Test]
        public void MismaLinea_NoEsTrasbordo()
        {
            var tarjeta = TarjetaConSaldo();
            Viajes.Pagar(_k, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(30));

            Viajes.Pagar(_k, tarjeta, out var segundo);

            Assert.That(segundo!.EsTransbordo, Is.False);
            Assert.That(segundo.Tarifa, Is.EqualTo(Reglas.TarifaBasica));
        }

        [TestCase(60, true)]
        [TestCase(61, false)]
        public void LaVentanaEsDeUnaHora(int minutos, bool esTrasbordo)
        {
            var tarjeta = TarjetaConSaldo();
            Viajes.Pagar(_k, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(minutos));

            Viajes.Pagar(_c101, tarjeta, out var segundo);

            Assert.That(segundo!.EsTransbordo, Is.EqualTo(esTrasbordo));
        }

        [Test]
        public void SinLimiteDeTrasbordosDentroDeLaHora()
        {
            var tarjeta = TarjetaConSaldo();
            Viajes.Pagar(_k, tarjeta, out _);

            foreach (var linea in new[] { _c101, _k, _c101 })
            {
                Reloj.Avanzar(TimeSpan.FromMinutes(15));
                Assert.That(Viajes.Pagar(linea, tarjeta, out var boleto), Is.True);
                Assert.That(boleto!.EsTransbordo, Is.True);
            }

            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - Reglas.TarifaBasica));
        }

        [Test]
        public void ElTrasbordoNoConsumeViajesDelDia_MedioBoleto()
        {
            var tarjeta = TarjetaConSaldo(2);
            Viajes.Pagar(_k, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(2));   // menos de 5 minutos: el trasbordo igual se permite
            Assert.That(Viajes.Pagar(_c101, tarjeta, out var trasbordo), Is.True);
            Assert.That(trasbordo!.EsTransbordo, Is.True);

            Reloj.Avanzar(TimeSpan.FromMinutes(61));
            Viajes.Pagar(_k, tarjeta, out var segundoPago);

            Assert.That(segundoPago!.EsTransbordo, Is.False);
            Assert.That(segundoPago.Tarifa, Is.EqualTo(Reglas.TarifaBasica / 2));   // sigue siendo su 2.º viaje
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void TodasLasTarjetasPuedenTrasbordar(int tipoId)
        {
            var tarjeta = TarjetaConSaldo(tipoId);
            Viajes.Pagar(_k, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(10));

            Viajes.Pagar(_c101, tarjeta, out var segundo);

            Assert.That(segundo!.EsTransbordo, Is.True);
        }

        [Test]
        public void EsTransbordo_SePersisteEnElBoleto()
        {
            var tarjeta = TarjetaConSaldo();
            Viajes.Pagar(_k, tarjeta, out _);
            Reloj.Avanzar(TimeSpan.FromMinutes(10));
            Viajes.Pagar(_c101, tarjeta, out var segundo);

            Db.ChangeTracker.Clear();

            Assert.That(Db.Boletos.Find(segundo!.Id)!.EsTransbordo, Is.True);
        }
    }
}