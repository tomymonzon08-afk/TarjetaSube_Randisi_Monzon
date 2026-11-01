using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TarjetaSube.Tests
{
    public abstract class BaseDeTests
    {
       protected static readonly DateTime FechaFija = new(2026, 9, 21, 10, 0, 0);
        private SqliteConnection _conexion = null!;

        protected TarjetaContext Db = null!;
        protected ProveedorDeFechaFalso Reloj = null!; 
        protected TarjetaServicio Tarjetas = null!;
        protected ColectivoServicio Colectivos = null!;
        protected ViajeServicio Viajes = null!;
        protected CargaServicio Cargas = null!;

        [SetUp]
        public void PrepararContexto()
        {
            _conexion = new SqliteConnection("Data Source=:memory:");
            _conexion.Open();
            var opciones = new DbContextOptionsBuilder<TarjetaContext>().UseSqlite(_conexion).Options;

            Db = new TarjetaContext(opciones);
            Db.Database.EnsureCreated();

            Reloj = new ProveedorDeFechaFalso(FechaFija); 
            Tarjetas = new TarjetaServicio(Db);
            Colectivos = new ColectivoServicio(Db);
            Viajes = new ViajeServicio(Db, Reloj);
            Cargas = new CargaServicio(Db);
        }

        [TearDown]
        public void LimpiarContexto() { Db.Dispose(); _conexion.Dispose(); }
    }
}
