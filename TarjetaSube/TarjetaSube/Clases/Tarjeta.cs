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

        public int TarjetaTipoId { get; set; } = Reglas.TipoNormalId;
        public TarjetaTipo TarjetaTipo { get; set; } = null!;

        public void AgregarSaldo(int monto)
        {
            if (!Reglas.CargasAceptadas.Contains(monto))
                throw new ArgumentException("Monto de carga no aceptado.");

            SaldoPendiente += monto;
            AcreditarCarga();
        }

        public bool PuedePagar(int monto) => Saldo - monto >= Reglas.SaldoMinimo;

        public void DescontarSaldo(int monto)
        {
            if (monto < 0)
                throw new ArgumentException("El monto no puede ser negativo.");
            if (!PuedePagar(monto))
                throw new InvalidOperationException("Saldo insuficiente.");

            Saldo -= monto;
        }

        public int CalcularTarifa(int viajesHoy = 0) => TarjetaTipo.CalcularTarifa(Reglas.TarifaBasica, viajesHoy);

        public int SaldoPendiente { get; set; }

        public void AcreditarCarga()
        {
            int espacioLibre = Math.Max(0, Reglas.SaldoMaximo - Saldo);
            int acreditable = Math.Min(espacioLibre, SaldoPendiente);

            Saldo += acreditable;
            SaldoPendiente -= acreditable;
        }

    }
}