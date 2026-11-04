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
    }
}