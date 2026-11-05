namespace TarjetaSube.Tests
{
    public class AlquilerBiciDevolucionTests
    {
        private static readonly DateTime Lunes10 = new(2026, 9, 21, 10, 0, 0);

        private static AlquilerBici Alquiler(Tarjeta tarjeta) =>
        new() { FechaRetiro = Lunes10, Tarjeta = tarjeta };

        [TestCase(59, false, 0)]
        [TestCase(60, false, 0)] 
        [TestCase(61, true, 1)]
        [TestCase(180, true, 1)] 
        public void Devolver_SegunLosMinutosDeUso_MultaSoloSiPasaDe60(int minutos,
        bool excedio, int multas)
        {
            var tarjeta = new Tarjeta();
            var alquiler = Alquiler(tarjeta);
            var resultado = alquiler.Devolver(Lunes10.AddMinutes(minutos));
            Assert.That(resultado, Is.True);
            Assert.That(alquiler.FechaDevolucion,
            Is.EqualTo(Lunes10.AddMinutes(minutos)));
            Assert.That(alquiler.ExcedioElTiempo, Is.EqualTo(excedio));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(multas));
        }

        [Test]
        public void Devolver_BiciYaDevuelta_DevuelveFalseYNoSumaOtraMulta()
        {
            var tarjeta = new Tarjeta();

            var alquiler = Alquiler(tarjeta);
            alquiler.Devolver(Lunes10.AddMinutes(90));
            var resultado = alquiler.Devolver(Lunes10.AddMinutes(200));
            Assert.That(resultado, Is.False);
            Assert.That(alquiler.FechaDevolucion,
            Is.EqualTo(Lunes10.AddMinutes(90)));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1));
        }

        [Test]
        public void Devolver_VariosAlquileresConDemora_AcumulaUnaMultaPorCada()
        {
            var tarjeta = new Tarjeta();
            Alquiler(tarjeta).Devolver(Lunes10.AddMinutes(90));
            Alquiler(tarjeta).Devolver(Lunes10.AddMinutes(120));
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(2));
        }

        [Test]
        public void Devolver_ConDemora_NoDescuentaSaldoEnEseMomento()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };
            Alquiler(tarjeta).Devolver(Lunes10.AddMinutes(90));
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000));
        }
    }
}