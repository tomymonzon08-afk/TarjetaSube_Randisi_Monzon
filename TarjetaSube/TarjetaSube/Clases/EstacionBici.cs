namespace TarjetaSube
{
    public class EstacionBici
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Recaudacion { get; set; }
        public List<AlquilerBici> Alquileres { get; set; } = new();
    }
}