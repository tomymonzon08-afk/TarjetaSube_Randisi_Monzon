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

        public static Tarjeta Crear(int numero, string dueno)
        {
            if (numero <= 0)
                throw new ArgumentException("El número de tarjeta no es válido.");
            if (string.IsNullOrWhiteSpace(dueno))
                throw new ArgumentException("El dueño es obligatorio.");
            if (Contexto.Db.Tarjetas.Any(t => t.Numero == numero))
                throw new InvalidOperationException("Ya existe una tarjeta con ese número.");

            var tarjeta = new Tarjeta { Numero = numero, Dueno = dueno };
            Contexto.Db.Tarjetas.Add(tarjeta);
            Contexto.Db.SaveChanges();
            return tarjeta;
        }

        public void Modificar(string dueno)
        {
            if (string.IsNullOrWhiteSpace(dueno))
                throw new ArgumentException("El dueño es obligatorio.");

            Dueno = dueno;
            Contexto.Db.SaveChanges();
        }

        public void Eliminar()
        {
            if (Contexto.Db.Boletos.Any(b => b.TarjetaId == Id))
                throw new InvalidOperationException("No se puede eliminar una tarjeta con boletos asociados.");

            Contexto.Db.Tarjetas.Remove(this);
            Contexto.Db.SaveChanges();
        }

        public void AgregarSaldo(int monto)
        {
            if (!Reglas.CargasAceptadas.Contains(monto))
                throw new ArgumentException("Monto de carga no aceptado.");
            if (Saldo + monto > Reglas.SaldoMaximo)
                throw new InvalidOperationException("La carga supera el saldo máximo permitido.");

            Saldo += monto;
        }

        public void DescontarSaldo(int monto)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser positivo.");
            if (Saldo < monto)
                throw new InvalidOperationException("Saldo insuficiente.");

            Saldo -= monto;
        }

    }
}