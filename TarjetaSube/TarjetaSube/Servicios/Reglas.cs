namespace TarjetaSube
{
    public static class Reglas
    {
        public const int TarifaBasica = 1580;
        public const int SaldoMaximo = 56000;
        public const int TipoNormalId = 1;
        public const int SaldoMinimo = -2000;
        public static readonly int[] CargasAceptadas =
            { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };
            public const int TarifaInterurbana = 3000;
            public const int MinutosVentanaTrasbordo = 60;

            public static readonly FranjaHoraria FranjaFranquicias = new(DayOfWeek.Monday, DayOfWeek.Friday, 6, 22);
            public static readonly FranjaHoraria FranjaTrasbordos = new(DayOfWeek.Monday, DayOfWeek.Saturday, 7, 22);
            public static readonly (int DesdeViaje, int Descuento)[] TramosUsoFrecuente = { (80, 25), (30, 20) };

    }
}