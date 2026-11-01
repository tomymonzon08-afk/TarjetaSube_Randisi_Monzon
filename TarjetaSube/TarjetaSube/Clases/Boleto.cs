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

        public string TipoTarjeta { get; set; } = string.Empty;
        public string Linea { get; set; } = string.Empty;
        public int SaldoRestante { get; set; }

        public int TotalAbonado => Tarifa;
        public int PlusAdeudado => SaldoRestante < 0 ? -SaldoRestante : 0;

    }
}