namespace TarjetaSube
{
    public class CargaServicio
    {
        private readonly TarjetaContext _db;
        public CargaServicio(TarjetaContext db) => _db = db;

        public void Cargar(Tarjeta tarjeta, int monto)
        {
            tarjeta.AgregarSaldo(monto);
            _db.SaveChanges();
        }
    }
}