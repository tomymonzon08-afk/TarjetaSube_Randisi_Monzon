using System;


namespace TarjetaSube { 
	public class Colectivo{
		public int Id { get; set; }
		public string Linea { get; set; } = string.Empty;
		public int Recaudacion { get; set; }
		public List<Boleto> Boletos { get; set; } = new();

        public bool EsInterurbana { get; set; }
        public int TarifaBase => EsInterurbana ? Reglas.TarifaInterurbana : Reglas.TarifaBasica;

        public bool PagarCon(Tarjeta tarjeta, DateTime fecha, HistorialDeViajes historial, out Boleto? boleto)
        {
            boleto = null;
            bool esTrasbordo = ReglaTrasbordo.Aplica(fecha, Linea, historial);
            int tarifa = 0;
            if (!esTrasbordo)
            {
                if (!tarjeta.TarjetaTipo.PermiteViajar(fecha, historial.UltimoViaje))
                    return false;
                tarifa = tarjeta.CalcularTarifa(historial.ViajesHoy, TarifaBase, historial.ViajesDelMes);

                if (!tarjeta.PuedePagar(tarifa))
                    return false;
            }
            tarjeta.DescontarSaldo(tarifa);
            tarjeta.AcreditarCarga();
            Recaudacion += tarifa;
            boleto = new Boleto
            {
                FechaHora = fecha,
                Tarifa = tarifa,
                Tarjeta = tarjeta,
                Colectivo = this,
                TipoTarjeta = tarjeta.TarjetaTipo.Nombre,
                Linea = Linea,
                SaldoRestante = tarjeta.Saldo,
                EsTransbordo = esTrasbordo
            };
            return true;
        }

    }
}
