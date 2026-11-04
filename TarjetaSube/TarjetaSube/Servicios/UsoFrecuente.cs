namespace TarjetaSube
{
    public static class UsoFrecuente
    {
        public static int PorcentajeDescuento(int numeroDeViaje)
        {
            foreach (var (desdeViaje, descuento) in Reglas.TramosUsoFrecuente)
                if (numeroDeViaje >= desdeViaje)
                    return descuento;
            return 0;
        }
    }
}
