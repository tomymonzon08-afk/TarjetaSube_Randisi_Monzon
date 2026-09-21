namespace TarjetaSube
{
    public static class Contexto
    {
        private static TarjetaContext? _db;

        public static TarjetaContext Db
        {
            get => _db ??= new TarjetaContext();
            set => _db = value;
        }

        public static IProveedorDeFecha Fecha { get; set; } = new ProveedorDeFechaSistema();
    }
}