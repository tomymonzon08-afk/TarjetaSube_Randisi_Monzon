namespace TarjetaSube
{
    public class TarjetaTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PorcentajeTarifa { get; set; }
        public List<Tarjeta> Tarjetas { get; set; } = new();

        public int CalcularTarifa(int tarifaBase) => tarifaBase * PorcentajeTarifa / 100;
    }
}