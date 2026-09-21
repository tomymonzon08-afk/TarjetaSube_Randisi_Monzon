using System;
namespace TarjetaSube
{
    public class Boleto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int Tarifa { get; set; } = Reglas.TarifaBasica;
        public int TarjetaId { get; set; }
        public int ColectivoId { get; set; }

        public Tarjeta Tarjeta { get; set; } = null!;
        public Colectivo Colectivo { get; set; } = null!;

    }
}