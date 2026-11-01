using System;


namespace TarjetaSube { 
	public class Colectivo{
		public int Id { get; set; }
		public string Linea { get; set; } = string.Empty;
		public int Recaudacion { get; set; }
		public List<Boleto> Boletos { get; set; } = new();

        public bool PagarCon(Tarjeta tarjeta, DateTime fecha, out Boleto? boleto)
        {
            boleto = null;
            int tarifa = tarjeta.CalcularTarifa();

            if (!tarjeta.PuedePagar(tarifa))
                return false;

            tarjeta.DescontarSaldo(tarifa);

            tarjeta.AcreditarCarga();

            Recaudacion += tarifa;
            boleto = new Boleto { FechaHora = fecha, Tarifa = tarifa, Tarjeta = tarjeta, Colectivo = this };
            return true;
        }

    }
}
