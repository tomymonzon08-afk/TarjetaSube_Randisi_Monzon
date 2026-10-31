using TarjetaSube;

Contexto.Db.Database.EnsureDeleted();
Contexto.Db.Database.EnsureCreated();

var tarjeta = Tarjeta.Crear(123456, "Ana");
var colectivo = Colectivo.Crear("K");

Console.WriteLine($"Saldo inicial: ${tarjeta.Saldo}");

colectivo.CargarTarjeta(tarjeta, 5000);
Console.WriteLine($"Saldo tras cargar $5000: ${tarjeta.Saldo}");

try
{
    colectivo.CargarTarjeta(tarjeta, 1000);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Carga inválida rechazada: {ex.Message}");
}

for (int i = 1; i <= 4; i++)
{
    if (colectivo.PagarCon(tarjeta, out var boleto))
        Console.WriteLine($"Viaje {i}: boleto #{boleto!.Id}, tarifa ${boleto.Tarifa}, saldo ${tarjeta.Saldo}");
    else
        Console.WriteLine($"Viaje {i}: rechazado. Saldo ${tarjeta.Saldo}");

}

Console.WriteLine($"Recaudación de la línea {colectivo.Linea}: ${colectivo.Recaudacion}");
Console.WriteLine($"Boletos en la base: {Contexto.Db.Boletos.Count()}");

Contexto.Db.Dispose();