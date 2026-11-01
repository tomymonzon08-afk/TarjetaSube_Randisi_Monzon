using System;


namespace TarjetaSube { 
	public class Colectivo{
		public int Id { get; set; }
		public string Linea { get; set; } = string.Empty;
		public int Recaudacion { get; set; }
		public List<Boleto> Boletos { get; set; } = new();

        public bool PagarCon(Tarjeta tarjeta, DateTime fecha, HistorialDeViajes historial, out Boleto? boleto)
        {
            boleto = null;

            if (!tarjeta.TarjetaTipo.PermiteViajar(fecha, historial.UltimoViaje))
                return false;

            int tarifa = tarjeta.CalcularTarifa(historial.ViajesHoy);
            if (!tarjeta.PuedePagar(tarifa))
                return false;

            tarjeta.DescontarSaldo(tarifa);
            Recaudacion += tarifa;
            boleto = new Boleto { FechaHora = fecha, Tarifa = tarifa, Tarjeta = tarjeta, Colectivo = this };
            return true;
        }


    }
}
