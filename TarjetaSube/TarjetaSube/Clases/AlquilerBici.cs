namespace TarjetaSube
{
    public class AlquilerBici
    {
        public int Id { get; set; }
        public DateTime FechaRetiro { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public bool PagoElDia { get; set; }
        public bool ExcedioElTiempo { get; set; }
        public decimal TarifaCobrada { get; set; }
        public int MultasCobradas { get; set; }
        public int TarjetaId { get; set; }

        public int EstacionId { get; set; }
        public Tarjeta Tarjeta { get; set; } = null!;
        public EstacionBici Estacion { get; set; } = null!;
        public decimal TotalAbonado => TarifaCobrada + MultasCobradas *
        Reglas.MultaBici;
    }
}