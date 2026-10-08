using System;
using System.Collections.Generic;
using System.Text;

namespace TarjetaSube.Tests
{
    public class ProveedorDeFechaFalso : IProveedorDeFecha
    {
        private readonly DateTime _fecha;

        public ProveedorDeFechaFalso(DateTime fecha) => _fecha = fecha;

        public DateTime Ahora() => _fecha;
    }
}
