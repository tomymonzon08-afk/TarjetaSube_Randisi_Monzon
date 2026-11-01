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

            if (!colectivo.PagarCon(tarjeta, _fecha.Ahora(), out boleto))
                return false;

            _db.Boletos.Add(boleto!);
            _db.SaveChanges();
            return true;
        }
    }
}