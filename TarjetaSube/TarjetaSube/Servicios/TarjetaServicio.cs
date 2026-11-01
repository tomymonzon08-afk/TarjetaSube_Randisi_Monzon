namespace TarjetaSube
{
    public class TarjetaServicio
    {
        private readonly TarjetaContext _db;
        public TarjetaServicio(TarjetaContext db) => _db = db;

        public Tarjeta Crear(int numero, string dueno, int tipoId = Reglas.TipoNormalId)
        {
            if (numero <= 0)
                throw new ArgumentException("El número de tarjeta no es válido.");
            if (string.IsNullOrWhiteSpace(dueno))
                throw new ArgumentException("El dueño es obligatorio.");
            if (_db.Tarjetas.Any(t => t.Numero == numero))
                throw new InvalidOperationException("Ya existe una tarjeta con ese número.");

            var tipo = _db.TarjetaTipos.Find(tipoId)
                ?? throw new ArgumentException("El tipo de tarjeta no existe.");

            var tarjeta = new Tarjeta { Numero = numero, Dueno = dueno, TarjetaTipo = tipo };
            _db.Tarjetas.Add(tarjeta);
            _db.SaveChanges();
            return tarjeta;
        }

        public void Modificar(Tarjeta tarjeta, string dueno)
        {
            if (string.IsNullOrWhiteSpace(dueno))
                throw new ArgumentException("El dueño es obligatorio.");

            tarjeta.Dueno = dueno;
            _db.SaveChanges();
        }

        public void Eliminar(Tarjeta tarjeta)
        {
            if (_db.Boletos.Any(b => b.TarjetaId == tarjeta.Id))
                throw new InvalidOperationException("No se puede eliminar una tarjeta con boletos asociados.");

            _db.Tarjetas.Remove(tarjeta);
            _db.SaveChanges();
        }
    }
}