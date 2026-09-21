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

        public static Boleto Registrar(Tarjeta tarjeta, Colectivo colectivo, int tarifa)
        {
            var boleto = new Boleto
            {
                FechaHora = Contexto.Fecha.Ahora(),
                Tarifa = tarifa,
                Tarjeta = tarjeta,
                Colectivo = colectivo
            };

            Contexto.Db.Boletos.Add(boleto);
            Contexto.Db.SaveChanges();
            return boleto;
        }
    }
}