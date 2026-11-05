namespace TarjetaSube.Tests
{
    public class DevolucionBiciServicioTests : BaseDeTests
    {
        private AlquilerBici AlquilerEnCurso(Tarjeta tarjeta, EstacionBici estacion)
        {
            var alquiler = new AlquilerBici
            {
                FechaRetiro = Reloj.Ahora(),
                PagoElDia = true,
                TarifaCobrada = Reglas.TarifaBici,
                Tarjeta = tarjeta,
                Estacion = estacion
            };
            Db.AlquileresBici.Add(alquiler);
            Db.SaveChanges();
            return alquiler;
        }

        [Test]
        public void Devolver_SinDemora_GuardaLaDevolucionYNoMulta()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var alquiler = AlquilerEnCurso(tarjeta, Estaciones.Crear("Centro"));
            Reloj.Avanzar(TimeSpan.FromMinutes(45));
            var resultado = Devoluciones.Devolver(alquiler);
            Assert.That(resultado, Is.True);
            Db.ChangeTracker.Clear();
            var guardado = Db.AlquileresBici.Find(alquiler.Id)!;
            Assert.That(guardado.FechaDevolucion,
            Is.EqualTo(FechaFija.AddMinutes(45)));
            Assert.That(guardado.ExcedioElTiempo, Is.False);
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.MultasBiciPendientes,
            Is.EqualTo(0));
        }

        [Test]
        public void Devolver_ConDemora_PersisteLaMultaEnLaTarjeta()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var alquiler = AlquilerEnCurso(tarjeta, Estaciones.Crear("Centro"));
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(alquiler);
            Db.ChangeTracker.Clear();
            Assert.That(Db.AlquileresBici.Find(alquiler.Id)!.ExcedioElTiempo,
            Is.True);
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.MultasBiciPendientes,
            Is.EqualTo(1));
        }

        [Test]
        public void
        Devolver_AlquilerLeidoDeLaBase_FuncionaAunqueNoTengaLaTarjetaCargada()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var alquiler = AlquilerEnCurso(tarjeta, Estaciones.Crear("Centro"));
            Db.ChangeTracker.Clear();
            var leido = Db.AlquileresBici.Find(alquiler.Id)!;
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            var resultado = Devoluciones.Devolver(leido);
            Assert.That(resultado, Is.True);
            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.MultasBiciPendientes,
            Is.EqualTo(1));
        }

        [Test]
        public void Devolver_BiciYaDevuelta_SeRechazaYNoSumaOtraMulta()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var alquiler = AlquilerEnCurso(tarjeta, Estaciones.Crear("Centro"));
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(alquiler);
            Assert.That(Devoluciones.Devolver(alquiler), Is.False);
            Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1));
        }

        [Test]
        public void Devolver_DosAlquileresConDemoraElMismoDia_AcumulaDosMultas()
        {
            var tarjeta = Tarjetas.Crear(1, "Ana");
            var estacion = Estaciones.Crear("Centro");
            var primero = AlquilerEnCurso(tarjeta, estacion);
            Reloj.Avanzar(TimeSpan.FromMinutes(90));
            Devoluciones.Devolver(primero);
            var segundo = AlquilerEnCurso(tarjeta, estacion);
            Reloj.Avanzar(TimeSpan.FromMinutes(75));
            Devoluciones.Devolver(segundo);
            Db.ChangeTracker.Clear();
            Assert.That(Db.Tarjetas.Find(tarjeta.Id)!.MultasBiciPendientes,
            Is.EqualTo(2));
        }
    }
}