using TarjetaSube.Tests;

namespace TarjetaSube
{
    public class ColectivoServicioTests : BaseDeTests
    {
        [Test]
        public void Crear_LineaValida_GuardaConRecaudacionCero()
        {
            var colectivo = Colectivos.Crear("K");

            Db.ChangeTracker.Clear();
            var guardado = Db.Colectivos.Find(colectivo.Id);

            Assert.That(guardado, Is.Not.Null);
            Assert.That(guardado!.Linea, Is.EqualTo("K"));
            Assert.That(guardado.Recaudacion, Is.EqualTo(0));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void Crear_LineaVacia_LanzaExcepcion(string linea)
        {
            Assert.Throws<ArgumentException>(() => Colectivos.Crear(linea));
        }

        [Test]
        public void Modificar_LineaValida_ActualizaYPersiste()
        {
            var colectivo = Colectivos.Crear("K");

            Colectivos.Modificar(colectivo, "101");

            Db.ChangeTracker.Clear();
            Assert.That(Db.Colectivos.Find(colectivo.Id)!.Linea, Is.EqualTo("101"));
        }

        [Test]
        public void Modificar_LineaVacia_LanzaExcepcionYNoCambia()
        {
            var colectivo = Colectivos.Crear("K");

            Assert.Throws<ArgumentException>(() => Colectivos.Modificar(colectivo,"K"));
            Assert.That(colectivo.Linea, Is.EqualTo("K"));
        }

        [Test]
        public void Eliminar_SinBoletos_BorraElColectivo()
        {
            var colectivo = Colectivos.Crear("K");

            Colectivos.Eliminar(colectivo);

            Assert.That(Db.Colectivos.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Eliminar_ConBoletos_LanzaExcepcionYNoBorra()
        {
            var colectivo = Colectivos.Crear("K");
            var tarjeta = Tarjetas.Crear(1, "Ana");
            tarjeta.AgregarSaldo(5000);
            Viajes.Pagar(colectivo,tarjeta, out _);;

            Assert.Throws<InvalidOperationException>(() => Colectivos.Eliminar(colectivo));
            Assert.That(Db.Colectivos.Count(), Is.EqualTo(1));
        }
    }
}