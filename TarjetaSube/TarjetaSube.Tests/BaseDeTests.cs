using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TarjetaSube.Tests
{
    public abstract class BaseDeTests
    {
        protected static readonly DateTime FechaFija = new(2026, 9, 21, 10, 0, 0);

        private SqliteConnection _conexion = null!;

        protected TarjetaContext Db = null!;
        protected TarjetaServicio Tarjetas = null!;
        protected ColectivoServicio Colectivos = null!;
        protected ViajeServicio Viajes = null!;      // nuevo
        protected CargaServicio Cargas = null!;      // nuevo

        [SetUp]
        public void PrepararContexto()
        {
            _conexion = new SqliteConnection("Data Source=:memory:");
            _conexion.Open();

            var opciones = new DbContextOptionsBuilder<TarjetaContext>()
                .UseSqlite(_conexion)
                .Options;

            Contexto.Db = new TarjetaContext(opciones);
            Contexto.Db.Database.EnsureCreated();
            Contexto.Fecha = new ProveedorDeFechaFalso(FechaFija);

            Db = Contexto.Db;
            Tarjetas = new TarjetaServicio(Db);
            Colectivos = new ColectivoServicio(Db);
            Viajes = new ViajeServicio(Db, Contexto.Fecha);   // nuevo
            Cargas = new CargaServicio(Db);                   // nuevo
        }

        [TearDown]
        public void LimpiarContexto()
        {
            Contexto.Db.Dispose();
            _conexion.Dispose();
        }
    }
}
