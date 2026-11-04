using System;

namespace TarjetaSube
{
	public class Tarjeta
	{
		public int Id { get; set; }
		public int Numero { get; set; }
		public decimal Saldo { get; set; }

        public decimal SaldoPendiente { get; set; }
		public string Dueno { get; set; }=string.Empty;
		public List<Boleto> Boletos { get; set; } = new();

        public int TarjetaTipoId { get; set; } = Reglas.TipoNormalId;
        public TarjetaTipo TarjetaTipo { get; set; } = null!;

        public void AgregarSaldo(int monto)
        {
            if (!Reglas.CargasAceptadas.Contains(monto))
                throw new ArgumentException("Monto de carga no aceptado.");

            SaldoPendiente += monto;
            AcreditarCarga();
        }

        public bool PuedePagar(decimal monto) => Saldo - monto >= Reglas.SaldoMinimo;
        public void DescontarSaldo(decimal monto)   
        {
            if (monto < 0)
                throw new ArgumentException("El monto no puede ser negativo.");
            if (!PuedePagar(monto))
                throw new InvalidOperationException("Saldo insuficiente.");

            Saldo -= monto;
        }

        public decimal CalcularTarifa(int viajesHoy = 0, decimal tarifaBase = Reglas.TarifaBasica, int viajesDelMes = 0)  => TarjetaTipo.CalcularTarifa(tarifaBase, viajesHoy, viajesDelMes);

        public void AcreditarCarga()
        {
            decimal espacioLibre = Math.Max(0m, Reglas.SaldoMaximo - Saldo);
            decimal acreditable = Math.Min(espacioLibre, SaldoPendiente);

            Saldo += acreditable;
            SaldoPendiente -= acreditable;
        }

        public int MultasBiciPendientes { get; set; }

    }
}