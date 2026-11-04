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
    }
}