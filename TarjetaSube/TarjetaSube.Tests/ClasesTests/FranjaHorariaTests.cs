namespace TarjetaSube.Tests
{
    public class FranjaHorariaTests
    {
        [TestCase(2026, 9, 21, 6, 0, true)]    
        [TestCase(2026, 9, 21, 5, 59, false)]
        [TestCase(2026, 9, 21, 21, 59, true)]
        [TestCase(2026, 9, 21, 22, 0, false)]
        [TestCase(2026, 9, 25, 12, 0, true)]    
        [TestCase(2026, 9, 26, 12, 0, false)]   
        [TestCase(2026, 9, 27, 12, 0, false)]  
        public void FranjaFranquicias_Contiene(int anio, int mes, int dia, int hora, int minuto, bool esperado)
        {
            var fecha = new DateTime(anio, mes, dia, hora, minuto, 0);
            Assert.That(Reglas.FranjaFranquicias.Contiene(fecha), Is.EqualTo(esperado));
        }

        [TestCase(2026, 9, 21, 7, 0, true)]
        [TestCase(2026, 9, 21, 6, 59, false)]
        [TestCase(2026, 9, 21, 21, 59, true)]
        [TestCase(2026, 9, 21, 22, 0, false)]
        [TestCase(2026, 9, 26, 12, 0, true)]   
        [TestCase(2026, 9, 27, 12, 0, false)]   
        public void FranjaTrasbordos_Contiene(int anio, int mes, int dia, int hora, int minuto, bool esperado)
        {
            var fecha = new DateTime(anio, mes, dia, hora, minuto, 0);
            Assert.That(Reglas.FranjaTrasbordos.Contiene(fecha), Is.EqualTo(esperado));
        }
    }
}
