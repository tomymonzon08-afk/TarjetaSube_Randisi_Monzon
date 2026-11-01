namespace TarjetaSube
{
    public class ViajeServicio
    {
        private readonly TarjetaContext _db;
        private readonly IProveedorDeFecha _fecha;

        public ViajeServicio(TarjetaContext db, IProveedorDeFecha fecha)
        {
            _db = db;
            _fecha = fecha;
        }

        public bool Pagar(Colectivo colectivo, Tarjeta tarjeta, out Boleto? boleto)
        {
            _db.Entry(tarjeta).Reference(t => t.TarjetaTipo).Load();

            var ahora = _fecha.Ahora();
            var historial = ObtenerHistorial(tarjeta, ahora);

            if (!colectivo.PagarCon(tarjeta, ahora, historial, out boleto))
                return false;

            _db.Boletos.Add(boleto!);
            _db.SaveChanges();
            return true;
        }

        public HistorialDeViajes ObtenerHistorial(Tarjeta tarjeta, DateTime ahora)
        {
            var inicioDelDia = ahora.Date;
            var finDelDia = inicioDelDia.AddDays(1);
            var deLaTarjeta = _db.Boletos.Where(b => b.TarjetaId == tarjeta.Id);

            int viajesHoy = deLaTarjeta.Count(b => b.FechaHora >= inicioDelDia && b.FechaHora < finDelDia);
            DateTime? ultimo = deLaTarjeta
                .OrderByDescending(b => b.FechaHora)
                .Select(b => (DateTime?)b.FechaHora)
                .FirstOrDefault();

            return new HistorialDeViajes(viajesHoy, ultimo);
        }

    }
}