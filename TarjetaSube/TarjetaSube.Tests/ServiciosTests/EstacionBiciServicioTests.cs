namespace TarjetaSube.Tests
{
    public class EstacionBiciServicioTests : BaseDeTests
    {
        [Test]
        public void Crear_GuardaLaEstacionSinRecaudacion()
        {
            var estacion = Estaciones.Crear("Centro");
            Db.ChangeTracker.Clear();
            var guardada = Db.EstacionesBici.Find(estacion.Id)!;
            Assert.That(guardada.Nombre, Is.EqualTo("Centro"));
            Assert.That(guardada.Recaudacion, Is.EqualTo(0));
        }

        [TestCase("")]
        [TestCase(" ")]
        public void Crear_ConNombreVacio_LanzaArgumentException(string nombre)
        {
            Assert.Throws<ArgumentException>(() => Estaciones.Crear(nombre));
            Assert.That(Db.EstacionesBici.Count(), Is.EqualTo(0));
        }
    }
}