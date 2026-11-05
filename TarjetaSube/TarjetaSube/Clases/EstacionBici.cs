namespace TarjetaSube
{
    public class EstacionBici
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Recaudacion { get; set; }
        public List<AlquilerBici> Alquileres { get; set; } = new();

        public bool RetirarCon(Tarjeta tarjeta, DateTime fecha, HistorialBici historial, out AlquilerBici? alquiler)
        {
            alquiler = null;

            if (historial.TieneBiciEnUso)
                return false;

            decimal tarifa = 0;
            int multas = 0;

            if (!historial.YaPagoHoy)
            {
                tarifa = Reglas.TarifaBici;
                multas = tarjeta.MultasBiciPendientes;
                decimal total = tarifa + multas * Reglas.MultaBici;

                if (tarjeta.Saldo < total)
                    return false;

                tarjeta.DescontarSaldo(total);
                tarjeta.AcreditarCarga();
                tarjeta.MultasBiciPendientes = 0;
                Recaudacion += total;
            }

            alquiler = new AlquilerBici
            {
                FechaRetiro = fecha,
                TarifaCobrada = tarifa,
                MultasCobradas = multas,
                PagoElDia = !historial.YaPagoHoy,
                Tarjeta = tarjeta,
                Estacion = this
            };
            return true;
        }

    }
}