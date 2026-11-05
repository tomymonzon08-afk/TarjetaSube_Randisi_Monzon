namespace TarjetaSube
{
    public class RetiroBiciServicio
    {
        private readonly TarjetaContext _db;

        private readonly IProveedorDeFecha _fecha;

        public RetiroBiciServicio(TarjetaContext db, IProveedorDeFecha fecha)
        {
            _db = db;
            _fecha = fecha;
        }

        public bool Retirar(EstacionBici estacion, Tarjeta tarjeta, out AlquilerBici? alquiler)
        {
            var ahora = _fecha.Ahora();
            var historial = ObtenerHistorial(tarjeta, ahora);

            if (!estacion.RetirarCon(tarjeta, ahora, historial, out alquiler))
                return false;

            _db.AlquileresBici.Add(alquiler!);
            _db.SaveChanges();
            return true;
        }

        public HistorialBici ObtenerHistorial(Tarjeta tarjeta, DateTime ahora)
        {
            var inicioDelDia = ahora.Date;
            var finDelDia = inicioDelDia.AddDays(1);
            var deLaTarjeta = _db.AlquileresBici.Where(a => a.TarjetaId == tarjeta.Id);

            bool enUso = deLaTarjeta.Any(a => a.FechaDevolucion == null);
            bool pagoHoy = deLaTarjeta.Any(a => a.PagoElDia && a.FechaRetiro >= inicioDelDia && a.FechaRetiro < finDelDia);

            return new HistorialBici(enUso, pagoHoy);
        }

    }
}