using System;
namespace TarjetaSube
{
    public class Boleto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int TarjetaId { get; set; }
        public int ColectivoId { get; set; }

        public Tarjeta Tarjeta { get; set; } = null!;
        public Colectivo Colectivo { get; set; } = null!;

        public string TipoTarjeta { get; set; } = string.Empty;
        public string Linea { get; set; } = string.Empty;
        public decimal Tarifa { get; set; } = Reglas.TarifaBasica;
        public decimal SaldoRestante { get; set; }
        public decimal TotalAbonado => Tarifa;
        public decimal PlusAdeudado => SaldoRestante < 0 ? -SaldoRestante : 0;
        public bool EsTransbordo { get; set; }

    }
}