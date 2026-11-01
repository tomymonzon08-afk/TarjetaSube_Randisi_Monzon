using System;


namespace TarjetaSube { 
	public class Colectivo{
		public int Id { get; set; }
		public string Linea { get; set; } = string.Empty;
		public int Recaudacion { get; set; }
		public List<Boleto> Boletos { get; set; } = new();

        public bool PagarCon(Tarjeta tarjeta, out Boleto? boleto){
            boleto = null;
            Contexto.Db.Entry(tarjeta).Reference(t => t.TarjetaTipo).Load();
            int tarifa = tarjeta.CalcularTarifa();


            if (!tarjeta.PuedePagar(tarifa))
                return false;
                
            tarjeta.DescontarSaldo(tarifa);
            Recaudacion += tarifa;
            boleto = Boleto.Registrar(tarjeta, this, tarifa);
            return true;
        }


        public void CargarTarjeta(Tarjeta tarjeta, int monto)
        {
            tarjeta.AgregarSaldo(monto);
            Contexto.Db.SaveChanges();
        }

    }
}
