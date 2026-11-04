namespace TarjetaSube.Tests
{
    public class ReglaTrasbordoTests
    {
        private static readonly DateTime Lunes10 = new(2026, 9, 21, 10, 0, 0);
        private static HistorialDeViajes Historial(DateTime ultimoPagado, string linea) => new(1, ultimoPagado, 1, linea);
        [Test]
        public void Aplica_OtraLineaDentroDeLaHora_EsTrue()
        {
            Assert.That(ReglaTrasbordo.Aplica(Lunes10, "101", Historial(Lunes10.AddMinutes(-30), "K")), Is.True);
        }
        [TestCase("K")]
        [TestCase("k")]
        public void Aplica_MismaLinea_EsFalse(string ultimaLinea)
        {
            Assert.That(ReglaTrasbordo.Aplica(Lunes10, "K", Historial(Lunes10.AddMinutes(-30), ultimaLinea)), Is.False);
        }
        [Test]
        public void Aplica_SinHistorial_EsFalse()
        {
            Assert.That(ReglaTrasbordo.Aplica(Lunes10, "101", HistorialDeViajes.Vacio), Is.False);
        }
        [TestCase(60, true)]
        [TestCase(61, false)]
        public void Aplica_SegunLosMinutosDesdeElUltimoPagado(int minutos, bool esperado)
        {
            Assert.That(ReglaTrasbordo.Aplica(Lunes10, "101", Historial(Lunes10.AddMinutes(-minutos), "K")), Is.EqualTo(esperado));
        }
        [TestCase(2026, 9, 21, 6, 59, false)]
        [TestCase(2026, 9, 21, 7, 0, true)]
        [TestCase(2026, 9, 21, 21, 59, true)]
        [TestCase(2026, 9, 21, 22, 0, false)]
        [TestCase(2026, 9, 26, 12, 0, true)] 
        [TestCase(2026, 9, 27, 12, 0, false)] 
        public void Aplica_SegunDiaYHora(int anio, int mes, int dia, int hora, int minuto, bool esperado)
        {
            var fecha = new DateTime(anio, mes, dia, hora, minuto, 0);
            Assert.That(ReglaTrasbordo.Aplica(fecha, "101", Historial(fecha.AddMinutes(-10), "K")), Is.EqualTo(esperado));
        }
    }
}