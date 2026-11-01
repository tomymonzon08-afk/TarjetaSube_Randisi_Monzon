namespace TarjetaSube.Tests{

    public class TarjetaServicioTests:BaseDeTests
    {
        
        [Test]
        public void Crear_DatosValidos_GuardaLaTarjetaConSaldoCero()
        {
            var tarjeta = Tarjetas.Crear(123456789, "Ana");

            Db.ChangeTracker.Clear();
            var guardada = Db.Tarjetas.Find(tarjeta.Id);

            Assert.That(guardada, Is.Not.Null);
            Assert.That(guardada!.Numero, Is.EqualTo(123456789));
            Assert.That(guardada.Dueno, Is.EqualTo("Ana"));
            Assert.That(guardada.Saldo, Is.EqualTo(0));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void Crear_NumeroInvalido_LanzaExcepcion(int numero)
        {
            Assert.Throws<ArgumentException>(() => Tarjetas.Crear(numero, "Ana"));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void Crear_DuenoVacio_LanzaExcepcion(string dueno)
        {
            Assert.Throws<ArgumentException>(() => Tarjetas.Crear(1, dueno));
        }

        [Test]
        public void Crear_NumeroRepetido_LanzaExcepcion()
        {
            Tarjetas.Crear(1, "Ana");

            Assert.Throws<InvalidOperationException>(() => Tarjetas.Crear(1, "Beto"));
        }
        
        [Test]
        public void Crear_ConTipoMedioBoleto_GuardaElTipo()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana", 2);

            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.TarjetaTipoId, Is.EqualTo(2));
        }

        [Test]
        public void Crear_TipoInexistente_LanzaExcepcion()
        {
            Assert.Throws<ArgumentException>(() => Tarjetas.Crear(1, "Ana", 99));
        }

        [Test]
        public void Modificar_DuenoValido_ActualizaYPersiste()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Tarjetas.Modificar(tarjeta,"Beto");

            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.Dueno, Is.EqualTo("Beto"));
        }

        [Test]
        public void Modificar_DuenoVacio_LanzaExcepcionYNoCambia()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Assert.Throws<ArgumentException>(() => Tarjetas.Modificar(tarjeta," "));
            Assert.That(tarjeta.Dueno, Is.EqualTo("Ana"));
        }
        [Test]
        public void Eliminar_SinBoletos_BorraLaTarjeta()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");

            Tarjetas.Eliminar(tarjeta);

            Assert.That(Db.Tarjetas.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Eliminar_ConBoletos_LanzaExcepcionYNoBorra()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var colectivo = Colectivos.Crear("K");
            tarjeta.AgregarSaldo(5000);
            Viajes.Pagar(colectivo, tarjeta, out _);

            Assert.Throws<InvalidOperationException>(() => Tarjetas.Eliminar(tarjeta));
            Assert.That(Db.Tarjetas.Count(), Is.EqualTo(1));
        }

    }
}