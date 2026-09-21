using System;

namespace TarjetaSube
{
	public class Tarjeta
	{
		public int Id { get; set; }
		public int Numero { get; set; }
		public int Saldo { get; set; }
		public string Dueno { get; set; }=string.Empty;
		public List<Boleto> Boletos { get; set; } = new();
    }
}