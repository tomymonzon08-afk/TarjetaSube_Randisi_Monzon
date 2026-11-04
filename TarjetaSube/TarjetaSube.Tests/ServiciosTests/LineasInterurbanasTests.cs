namespace TarjetaSube.Tests
{
    public class LineasInterurbanasTests : BaseDeTests
    {
        [Test]
        public void Crear_LineaInterurbana_SePersisteComoInterurbana()
        {
            var galvez = Colectivos.Crear("Gálvez", true);
            Db.ChangeTracker.Clear();
            Assert.That(Db.Colectivos.Find(galvez.Id)!.EsInterurbana, Is.True);
        }
        [Test]
        public void Crear_SinIndicarElTipo_EsUrbana()
        {
            Assert.That(Colectivos.Crear("K").EsInterurbana, Is.False);
        }
        [TestCase(1, Reglas.TarifaInterurbana)]
        [TestCase(2, Reglas.TarifaInterurbana / 2)]
        [TestCase(3, 0)]
        [TestCase(4, 0)]
        public void Pagar_LineaInterurbana_CobraLaTarifaSegunElTipo(int tipoId, int esperado)
        {
            var galvez = Colectivos.Crear("Gálvez", true);
            var tarjeta = Tarjetas.Crear(1, "Ana", tipoId);
            tarjeta.AgregarSaldo(5000);
            Viajes.Pagar(galvez, tarjeta, out var boleto);
            Assert.That(boleto!.Tarifa, Is.EqualTo(esperado));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - esperado));
            Assert.That(galvez.Recaudacion, Is.EqualTo(esperado));
            Assert.That(boleto.Linea, Is.EqualTo("Gálvez"));
        }
        [Test]
        public void Pagar_BoletoGratuitoEnInterurbana_ElTercerViajeCobraLaTarifaInterurbana()
        {
            var galvez = Colectivos.Crear("Gálvez", true);
            var tarjeta = Tarjetas.Crear(1, "Ana", 3);
            tarjeta.AgregarSaldo(5000);
            Viajes.Pagar(galvez, tarjeta, out _);
            Viajes.Pagar(galvez, tarjeta, out _);
            Viajes.Pagar(galvez, tarjeta, out var tercero);
            Assert.That(tercero!.Tarifa, Is.EqualTo(Reglas.TarifaInterurbana));
        }
    }
}