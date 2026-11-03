namespace TarjetaSube.Tests
{
    public class TarjetaTipoFranjaTests
    {
        private static readonly DateTime Sabado = new(2026, 9, 26, 10, 0, 0);
        private static readonly DateTime Lunes = new(2026, 9, 21, 10, 0, 0);

        [Test]
        public void PermiteViajar_ConFranja_FueraDeHorarioEsFalse()
        {
            var tipo = new TarjetaTipo { UsaFranjaHoraria = true };

            Assert.That(tipo.PermiteViajar(Sabado, null), Is.False);
            Assert.That(tipo.PermiteViajar(Lunes, null), Is.True);
        }

        [Test]
        public void PermiteViajar_SinFranja_NoRestringePorHorario()
        {
            Assert.That(new TarjetaTipo().PermiteViajar(Sabado, null), Is.True);
        }
    }
}
