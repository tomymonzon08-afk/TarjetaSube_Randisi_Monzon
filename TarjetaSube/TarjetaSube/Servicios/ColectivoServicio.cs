namespace TarjetaSube
{
    public class ColectivoServicio
    {
        private readonly TarjetaContext _db;
        public ColectivoServicio(TarjetaContext db) => _db = db;

        public Colectivo Crear(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
                throw new ArgumentException("La línea es obligatoria.");

            var colectivo = new Colectivo { Linea = linea };
            _db.Colectivos.Add(colectivo);
            _db.SaveChanges();
            return colectivo;
        }

        public void Modificar(Colectivo colectivo, string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
                throw new ArgumentException("La línea es obligatoria.");

            colectivo.Linea = linea;
            _db.SaveChanges();
        }

        public void Eliminar(Colectivo colectivo)
        {
            if (_db.Boletos.Any(b => b.ColectivoId == colectivo.Id))
                throw new InvalidOperationException("No se puede eliminar un colectivo con boletos asociados.");

            _db.Colectivos.Remove(colectivo);
            _db.SaveChanges();
        }
    }
}