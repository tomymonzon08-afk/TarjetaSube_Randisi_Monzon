namespace TarjetaSube
{
    public class DevolucionBiciServicio
    {
        private readonly TarjetaContext _db;
        private readonly IProveedorDeFecha _fecha;

        public DevolucionBiciServicio(TarjetaContext db, IProveedorDeFecha fecha)
        {
            _db = db;
            _fecha = fecha;
        }

        public bool Devolver(AlquilerBici alquiler)
        {
            _db.Entry(alquiler).Reference(a => a.Tarjeta).Load();

            if (!alquiler.Devolver(_fecha.Ahora()))
                return false;

            _db.SaveChanges();
            return true;
        }
    }
}