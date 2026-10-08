using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TarjetaSube.Tests
{
    public abstract class BaseDeTests
    {
        protected static readonly DateTime FechaFija = new(2026, 9, 21, 10, 0, 0);

        private SqliteConnection _conexion = null!;

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
        }

        [TearDown]
        public void LimpiarContexto()
        {
            Contexto.Db.Dispose();
            _conexion.Dispose();
        }
    }
}
