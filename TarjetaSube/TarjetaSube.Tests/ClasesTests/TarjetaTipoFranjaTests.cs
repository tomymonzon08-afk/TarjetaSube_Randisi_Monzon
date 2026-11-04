namespace TarjetaSube.Tests
{
    public class TarjetaTipoFranjaTests
    {
        private static readonly DateTime Sabado12 = new(2026, 9, 26, 12, 0, 0);
        private static TarjetaTipo Franquicia() =>
new () {
            Nombre = "Medio boleto estudiantil", PorcentajeTarifa = 50,
UsaFranjaHoraria = true };
        [TestCase(2026, 9, 21, 6, 0, true)] 
        [TestCase(2026, 9, 21, 5, 59, false)]
        [TestCase(2026, 9, 21, 21, 59, true)]
        [TestCase(2026, 9, 21, 22, 0, false)]
        [TestCase(2026, 9, 25, 12, 0, true)] 
        [TestCase(2026, 9, 26, 12, 0, false)] 
        [TestCase(2026, 9, 27, 12, 0, false)] 
        public void PermiteViajar_ConFranja_SoloLunesAViernesDe6a22(int anio, int
        mes, int dia, int hora, int minuto, bool esperado)
        {
            var fecha = new DateTime(anio, mes, dia, hora, minuto, 0);
            Assert.That(Franquicia().PermiteViajar(fecha, null),
            Is.EqualTo(esperado));
        }
        [TestCase(2026, 9, 26, 12, 0)] 
        [TestCase(2026, 9, 27, 3, 0)] 
        [TestCase(2026, 9, 21, 23, 0)] 
        public void PermiteViajar_SinFranja_PermiteCualquierDiaYHora(int anio, int
        mes, int dia, int hora, int minuto)
        {
            var normal = new TarjetaTipo
            {
                Nombre = " Normal ",
                PorcentajeTarifa = 100
            };
            var fecha = new DateTime(anio, mes, dia, hora, minuto, 0);

            Assert.That(normal.PermiteViajar(fecha, null), Is.True);
        }
        [Test]
        public void
        PermiteViajar_FueraDeLaFranja_SeRechazaAunqueYaPasaronLosMinutos()
        {
            var tipo = new TarjetaTipo
            {
                MinutosEntreViajes = 5,
                UsaFranjaHoraria =
            true
            };
            Assert.That(tipo.PermiteViajar(Sabado12, Sabado12.AddMinutes(-30)),
            Is.False);
        }
        [Test]
        public void PagarCon_FranquiciaFueraDeLaFranja_DevuelveFalseYNoCambiaNada()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = Franquicia() };
            var resultado = colectivo.PagarCon(tarjeta, Sabado12,
            HistorialDeViajes.Vacio, out var boleto);
            Assert.That(resultado, Is.False);
            Assert.That(boleto, Is.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
            Assert.That(colectivo.Recaudacion, Is.EqualTo(0));
        }
        [Test]
        public void PagarCon_TarjetaNormalElSabado_Paga()
        {
            var colectivo = new Colectivo { Linea = "K" };
            var tarjeta = new Tarjeta
            {
                Saldo = 5000,
                TarjetaTipo = new TarjetaTipo
                {
                    Nombre = " Normal ",
                    PorcentajeTarifa = 100
                }
            };
            Assert.That(colectivo.PagarCon(tarjeta, Sabado12,
            HistorialDeViajes.Vacio, out _), Is.True);
        }
    }
}
