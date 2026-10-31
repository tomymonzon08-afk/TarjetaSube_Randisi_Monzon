namespace TarjetaSube
{
    public class ProveedorDeFechaSistema : IProveedorDeFecha
{
    public DateTime Ahora() => DateTime.Now;
}
}