namespace TarjetaSube.Tests
{
	public class HistorialBiciTests
	{
		[Test]
		public void Vacio_NoTieneBiciEnUsoNiPagoHoy()
		{
			Assert.That(HistorialBici.Vacio.TieneBiciEnUso, Is.False);
			Assert.That(HistorialBici.Vacio.YaPagoHoy, Is.False);
		}

		[Test]
		public void Constructor_GuardaLosDosDatos()
		{
			var historial = new HistorialBici(true, false);
			Assert.That(historial.TieneBiciEnUso, Is.True);
			Assert.That(historial.YaPagoHoy, Is.False);
		}
	}
}