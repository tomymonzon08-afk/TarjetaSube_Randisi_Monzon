using System;


namespace TarjetaSube { 
	public class Colectivo{
		public int Id { get; set; }
		public string Linea { get; set; } = string.Empty;
		public int Recaudacion { get; set; }
		public List<Boleto> Boletos { get; set; } = new();

        public static Colectivo Crear(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
                throw new ArgumentException("La línea es obligatoria.");

            var colectivo = new Colectivo { Linea = linea };
            Contexto.Db.Colectivos.Add(colectivo);
            Contexto.Db.SaveChanges();
            return colectivo;
        }

        public void Modificar(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
                throw new ArgumentException("La línea es obligatoria.");

            Linea = linea;
            Contexto.Db.SaveChanges();
        }

        public void Eliminar()
        {
            if (Contexto.Db.Boletos.Any(b => b.ColectivoId == Id))
                throw new InvalidOperationException("No se puede eliminar un colectivo con boletos asociados.");

            Contexto.Db.Colectivos.Remove(this);
            Contexto.Db.SaveChanges();
        }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            tarjeta.DescontarSaldo(Reglas.TarifaBasica);  
            Recaudacion += Reglas.TarifaBasica;
            return Boleto.Registrar(tarjeta, this, Reglas.TarifaBasica);
        }

        public void CargarTarjeta(Tarjeta tarjeta, int monto)
        {
            tarjeta.AgregarSaldo(monto);
            Contexto.Db.SaveChanges();
        }

    }
}
