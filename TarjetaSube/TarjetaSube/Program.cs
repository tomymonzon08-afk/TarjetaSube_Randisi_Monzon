using TarjetaSube;

using var db = new TarjetaContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

var tarjetas = new TarjetaServicio(db);
var colectivos = new ColectivoServicio(db);
var cargas = new CargaServicio(db);
var viajes = new ViajeServicio(db, new ProveedorDeFechaSistema());

var tarjeta = tarjetas.Crear(123456, "Ana");
var colectivo = colectivos.Crear("K");

Console.WriteLine($"Saldo inicial: ${tarjeta.Saldo}");

cargas.Cargar(tarjeta, 5000);
Console.WriteLine($"Saldo tras cargar $5000: ${tarjeta.Saldo}");

try
{
    cargas.Cargar(tarjeta, 1000);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Carga inválida rechazada: {ex.Message}");
}

for (int i = 1; i <= 5; i++)
{
    if (viajes.Pagar(colectivo, tarjeta, out var boleto))
        Console.WriteLine($"Viaje {i}: {boleto!.TipoTarjeta}, línea {boleto.Linea}, abonó ${boleto.TotalAbonado}, saldo restante ${boleto.SaldoRestante}");

    else
        Console.WriteLine($"Viaje {i}: rechazado. Saldo ${tarjeta.Saldo}");

}

Console.WriteLine($"Recaudación de la línea {colectivo.Linea}: ${colectivo.Recaudacion}");
Console.WriteLine($"Boletos en la base: {db.Boletos.Count()}");

db.Dispose();