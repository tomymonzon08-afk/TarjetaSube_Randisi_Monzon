namespace TarjetaSube
{
    public class EstacionBiciServicio
    {
        private readonly TarjetaContext _db;

        public EstacionBiciServicio(TarjetaContext db) => _db = db;

        public EstacionBici Crear(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            var estacion = new EstacionBici { Nombre = nombre };
            _db.EstacionesBici.Add(estacion);
            _db.SaveChanges();
            return estacion;
        }
    }
}