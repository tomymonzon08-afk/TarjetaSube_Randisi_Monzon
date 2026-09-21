using System;


namespace TarjetaSube { 
	public class Colectivo{
		public int Id { get; set; }
		public string Linea { get; set; } = string.Empty;
		public int Recaudacion { get; set; }
		public List<Boleto> Boletos { get; set; } = new();
	}
}
