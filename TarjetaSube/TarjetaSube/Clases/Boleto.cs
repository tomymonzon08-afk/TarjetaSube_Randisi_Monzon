using System;
namespace TarjetaSube
{
    public class Boleto
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Tarifa { get; set; }
        public int TarjetaId { get; set; }
        public int ColectivoId { get; set; }

        public Tarjeta tarjeta { get; set; }
        public Colectivo colectivo { get; set; }

    }
}