using System.Globalization;
using TarjetaSube;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var ar = new CultureInfo("es-AR");

using var db = new TarjetaContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();


var reloj = new ProveedorDeFechaManual(new DateTime(2026, 9, 21, 10, 0, 0));   

var tarjetas = new TarjetaServicio(db);
var colectivos = new ColectivoServicio(db);
var cargas = new CargaServicio(db);
var viajes = new ViajeServicio(db, reloj);
var estaciones = new EstacionBiciServicio(db);
var retiros = new RetiroBiciServicio(db, reloj);
var devoluciones = new DevolucionBiciServicio(db, reloj);

var k = colectivos.Crear("K");
var c101 = colectivos.Crear("101");
var galvez = colectivos.Crear("Gálvez", esInterurbana: true);
var estacion = estaciones.Crear("Centro");

string M(decimal monto) => (monto < 0 ? "-$" : "$") + Math.Abs(monto).ToString("N2", ar);
string Hora() => reloj.Ahora().ToString("ddd dd/MM HH:mm", ar);
void Titulo(string texto) { Console.WriteLine(); Console.WriteLine($"=== {texto} ==="); }
void Nota(string texto) => Console.WriteLine($"  · {texto}");

void Cargar(Tarjeta tarj, int monto)
{
    try
    {
        cargas.Cargar(tarj, monto);
        Console.WriteLine($"  {tarj.Dueno} carga {M(monto)} → saldo {M(tarj.Saldo)}, pendiente de acreditar {M(tarj.SaldoPendiente)}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"  {tarj.Dueno} carga {M(monto)} → RECHAZADA: {ex.Message}");
    }
}

void Viajar(Tarjeta tarj, Colectivo linea)
{
    if (viajes.Pagar(linea, tarj, out var boleto))
    {
        var trasbordo = boleto!.EsTransbordo ? " [TRASBORDO]" : "";
        var plus = boleto.PlusAdeudado > 0 ? $" (plus adeudado {M(boleto.PlusAdeudado)})" : "";
        Console.WriteLine($"  {Hora()} {tarj.Dueno} · {boleto.TipoTarjeta} · línea {boleto.Linea}: abonó {M(boleto.TotalAbonado)}{trasbordo}{plus} · saldo {M(boleto.SaldoRestante)}");
    }
    else
    {
        Console.WriteLine($"  {Hora()} {tarj.Dueno} · línea {linea.Linea}: RECHAZADO · saldo {M(tarj.Saldo)}");
    }
}

AlquilerBici? Retirar(Tarjeta tarj)
{
    if (retiros.Retirar(estacion, tarj, out var alquiler))
    {
        var detalle = alquiler!.TarifaCobrada == 0
            ? "no paga (ya pagó hoy)"
            : $"abonó {M(alquiler.TotalAbonado)} (tarifa {M(alquiler.TarifaCobrada)} + {alquiler.MultasCobradas} multa/s)";
        Console.WriteLine($"  {Hora()} {tarj.Dueno} retira una bici: {detalle} · saldo {M(tarj.Saldo)}");
        return alquiler;
    }

    Console.WriteLine($"  {Hora()} {tarj.Dueno} intenta retirar una bici: RECHAZADO · saldo {M(tarj.Saldo)}, multas pendientes {tarj.MultasBiciPendientes}");
    return null;
}

void Devolver(Tarjeta tarj, AlquilerBici? alquiler)
{
    if (alquiler is null)
        return;

    if (!devoluciones.Devolver(alquiler))
    {
        Console.WriteLine($"  {Hora()} {tarj.Dueno}: esa bici ya estaba devuelta");
        return;
    }

    var resultado = alquiler.ExcedioElTiempo ? "CON DEMORA (nueva multa)" : "a tiempo";
    Console.WriteLine($"  {Hora()} {tarj.Dueno} devuelve la bici {resultado} · multas pendientes {tarj.MultasBiciPendientes}");
}

Titulo("1. Tarjetas, tipos y cargas");

var ana = tarjetas.Crear(1001, "Ana");              
var beto = tarjetas.Crear(1002, "Beto", 2);        
var cami = tarjetas.Crear(1003, "Cami", 3);        
var dani = tarjetas.Crear(1004, "Dani", 4);    
var eva = tarjetas.Crear(1005, "Eva");             

foreach (var tarjetaCreada in new[] { ana, beto, cami, dani, eva })
    Console.WriteLine($"  Tarjeta #{tarjetaCreada.Numero} de {tarjetaCreada.Dueno}: {tarjetaCreada.TarjetaTipo.Nombre}");

Cargar(ana, 5000);
Cargar(ana, 1000);                          
Cargar(beto, 8000);
Cargar(cami, 3000);
Cargar(eva, 8000);

try
{
    tarjetas.Crear(1001, "Otra persona");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Crear tarjeta repetida → RECHAZADO: {ex.Message}");
}

Titulo("2. Saldo negativo (plus) y trasbordos (tarjeta Normal)");
Nota("Tarifa básica $2.150. Se puede quedar hasta en -$2.000.");

Viajar(ana, k);
reloj.Avanzar(TimeSpan.FromMinutes(10));
Viajar(ana, k);
reloj.Avanzar(TimeSpan.FromMinutes(10));
Viajar(ana, k);                                      
reloj.Avanzar(TimeSpan.FromMinutes(10));
Viajar(ana, k);                                      

Nota("Ana recarga y la carga descuenta lo que debía del plus:");
Cargar(ana, 5000);

reloj.Avanzar(TimeSpan.FromMinutes(5));
Nota("Otra línea dentro de la hora del último pago → trasbordo gratis:");
Viajar(ana, c101);

reloj.Avanzar(TimeSpan.FromMinutes(5));
Nota("La misma línea del último boleto no es trasbordo → paga:");
Viajar(ana, c101);

Titulo("3. Medio boleto estudiantil");
Nota("Mitad de precio, 5 minutos entre viajes, máximo 2 viajes con descuento por día.");

reloj.Fijar(new DateTime(2026, 9, 21, 11, 0, 0));
Viajar(beto, k);
Nota("Segundo intento inmediato (menos de 5 minutos):");
Viajar(beto, k);
reloj.Avanzar(TimeSpan.FromMinutes(5));
Viajar(beto, k);
reloj.Avanzar(TimeSpan.FromMinutes(10));
Nota("El tercer viaje del día ya paga la tarifa completa:");
Viajar(beto, k);

Titulo("4. Boleto gratuito y franquicia completa");

reloj.Fijar(new DateTime(2026, 9, 21, 12, 0, 0));
Nota("Boleto gratuito estudiantil: 2 viajes gratis por día, el tercero se cobra completo.");
Viajar(cami, k);
Viajar(cami, k);
Viajar(cami, k);
Nota("Franquicia completa: siempre gratis, sin límite de viajes.");
Viajar(dani, k);
Viajar(dani, k);
Viajar(dani, k);

Titulo("5. Línea interurbana (Gálvez, $3.000)");

reloj.Fijar(new DateTime(2026, 9, 22, 8, 0, 0));
Viajar(eva, galvez);
Nota("Las franquicias también viajan en interurbanas (medio boleto = mitad de $3.000):");
Viajar(beto, galvez);

Titulo("6. Franja horaria de las franquicias (lunes a viernes de 6 a 22)");

reloj.Fijar(new DateTime(2026, 9, 25, 21, 59, 0));   
Viajar(dani, k);
reloj.Avanzar(TimeSpan.FromMinutes(1));
Nota("A las 22:00 ya está fuera de la franja:");
Viajar(dani, k);

reloj.Fijar(new DateTime(2026, 9, 26, 12, 0, 0));   
Nota("El sábado ninguna franquicia puede pagar; la tarjeta Normal sí:");
Viajar(beto, k);
Viajar(cami, k);
Viajar(dani, k);
Viajar(eva, k);

Titulo("7. Boleto de uso frecuente (solo tarjeta Normal)");
Nota("Viaje 30 al 79: 20% de descuento. Desde el viaje 80: 25%. Los viajes anteriores del mes se simulan en la base.");

var fede = tarjetas.Crear(1006, "Fede");
Cargar(fede, 5000);

void SimularViajesPrevios(int cantidad, DateTime primero)
{
    for (int i = 0; i < cantidad; i++)
        db.Boletos.Add(new Boleto
        {
            FechaHora = primero.AddMinutes(i),
            Tarifa = Reglas.TarifaBasica,
            Tarjeta = fede,
            Colectivo = k,
            TipoTarjeta = "Normal",
            Linea = k.Linea
        });
    db.SaveChanges();
}

reloj.Fijar(new DateTime(2026, 9, 28, 10, 0, 0));
SimularViajesPrevios(29, new DateTime(2026, 9, 1, 8, 0, 0));
Nota("29 viajes previos en septiembre → este es el viaje 30:");
Viajar(fede, k);

SimularViajesPrevios(49, new DateTime(2026, 9, 2, 8, 0, 0));
reloj.Avanzar(TimeSpan.FromMinutes(10));
Nota("Con 79 viajes previos → este es el viaje 80 (25%, sin truncar):");
Viajar(fede, k);

Titulo("8. Saldo máximo ($56.000) y carga pendiente de acreditación");

var gina = tarjetas.Crear(1007, "Gina");
Cargar(gina, 30000);
Cargar(gina, 30000);                                 
reloj.Fijar(new DateTime(2026, 9, 28, 12, 0, 0));
Nota("Al viajar se libera lugar y se acredita lo pendiente:");
Viajar(gina, k);
Nota($"Pendiente de acreditar después del viaje: {M(gina.SaldoPendiente)}");

Titulo("9. Altas, modificaciones y bajas");

var temporal = tarjetas.Crear(9999, "Temporal");
tarjetas.Modificar(temporal, "Temporal modificada");
Console.WriteLine($"  Tarjeta #{temporal.Numero} ahora es de: {temporal.Dueno}");
tarjetas.Eliminar(temporal);
Console.WriteLine($"  Tarjeta eliminada. Tarjetas en la base: {db.Tarjetas.Count()}");

try
{
    tarjetas.Crear(0, "Número inválido");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  Crear tarjeta con número 0 → RECHAZADO: {ex.Message}");
}

try
{
    tarjetas.Eliminar(ana);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Eliminar la tarjeta de Ana → RECHAZADO: {ex.Message}");
}

var lineaTemporal = colectivos.Crear("Temporal");
colectivos.Modificar(lineaTemporal, "Línea 500");
Console.WriteLine($"  Colectivo creado y renombrado: {lineaTemporal.Linea}");
colectivos.Eliminar(lineaTemporal);

try
{
    colectivos.Eliminar(k);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Eliminar la línea K → RECHAZADO: {ex.Message}");
}

Titulo("10. Mi Bici Tu Bici");
Nota("$2.418,75 por día (se cobra en el primer retiro del día). Hasta 1 hora de uso; si se pasa, multa de $1.250 que se cobra en el próximo pago.");

Nota("Cualquier tarjeta paga lo mismo, sin franja ni descuentos (Dani, franquicia completa, un domingo de noche):");
Cargar(dani, 5000);
reloj.Fijar(new DateTime(2026, 9, 27, 23, 0, 0));    
var alquilerDani = Retirar(dani);
reloj.Avanzar(TimeSpan.FromMinutes(30));
Devolver(dani, alquilerDani);

var gus = tarjetas.Crear(1008, "Gus");
Cargar(gus, 10000);
reloj.Fijar(new DateTime(2026, 9, 29, 8, 0, 0));    
Nota("Gus: una bici por tarjeta a la vez.");
var alquiler1 = Retirar(gus);
Retirar(gus);                                      
reloj.Avanzar(TimeSpan.FromMinutes(45));
Devolver(gus, alquiler1);

reloj.Avanzar(TimeSpan.FromMinutes(15));
Nota("Segundo retiro el mismo día: no se vuelve a cobrar.");
var alquiler2 = Retirar(gus);
reloj.Avanzar(TimeSpan.FromMinutes(90));
Nota("Devuelve pasada la hora: multa.");
Devolver(gus, alquiler2);

reloj.Avanzar(TimeSpan.FromMinutes(30));
var alquiler3 = Retirar(gus);                       
reloj.Avanzar(TimeSpan.FromMinutes(75));
Nota("Otra devolución tardía el mismo día: las multas se acumulan.");
Devolver(gus, alquiler3);

reloj.Fijar(new DateTime(2026, 9, 30, 8, 0, 0));  
Nota("Al día siguiente se cobra la tarifa más todas las multas (2.418,75 + 2 × 1.250):");
var alquiler4 = Retirar(gus);
reloj.Avanzar(TimeSpan.FromMinutes(30));
Devolver(gus, alquiler4);

Nota("Saldo insuficiente (Hugo carga $2.000):");
var hugo = tarjetas.Crear(1009, "Hugo");
Cargar(hugo, 2000);
Retirar(hugo);

Nota("Saldo alcanza para la tarifa pero no para tarifa + multa (Ivo):");
var ivo = tarjetas.Crear(1010, "Ivo");
Cargar(ivo, 3000);
reloj.Fijar(new DateTime(2026, 10, 1, 8, 0, 0));
var alquilerIvo = Retirar(ivo);
reloj.Avanzar(TimeSpan.FromMinutes(90));
Devolver(ivo, alquilerIvo);
reloj.Fijar(new DateTime(2026, 10, 2, 8, 0, 0));
Retirar(ivo);                                        

Titulo("11. Resumen");

foreach (var linea in new[] { k, c101, galvez })
    Console.WriteLine($"  Línea {linea.Linea}: recaudó {M(linea.Recaudacion)}");

Console.WriteLine($"  Estación {estacion.Nombre}: recaudó {M(estacion.Recaudacion)}");
Console.WriteLine($"  Boletos en la base: {db.Boletos.Count()} ({db.Boletos.Count(b => b.EsTransbordo)} trasbordos; incluye 78 viajes simulados de Fede)");
Console.WriteLine($"  Alquileres de bici en la base: {db.AlquileresBici.Count()}");