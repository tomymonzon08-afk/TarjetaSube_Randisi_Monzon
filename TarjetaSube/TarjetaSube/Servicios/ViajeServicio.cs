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
            var inicioDelMes = new DateTime(ahora.Year, ahora.Month, 1);
            var finDelMes = inicioDelMes.AddMonths(1);

            var deLaTarjeta = _db.Boletos.Where(b => b.TarjetaId == tarjeta.Id);
            var pagados = deLaTarjeta.Where(b => !b.EsTransbordo);

            int viajesHoy = pagados.Count(b => b.FechaHora >= inicioDelDia && b.FechaHora < finDelDia);
            int viajesDelMes = pagados.Count(b => b.FechaHora >= inicioDelMes && b.FechaHora < finDelMes);
            DateTime? ultimoPagado = pagados
                .OrderByDescending(b => b.FechaHora)
                .Select(b => (DateTime?)b.FechaHora)
                .FirstOrDefault();
            string? lineaUltimoBoleto = deLaTarjeta
                .OrderByDescending(b => b.FechaHora).ThenByDescending(b => b.Id)
                .Select(b => b.Linea)
                .FirstOrDefault();

            return new HistorialDeViajes(viajesHoy, ultimoPagado, viajesDelMes, lineaUltimoBoleto);
        }

    }
}