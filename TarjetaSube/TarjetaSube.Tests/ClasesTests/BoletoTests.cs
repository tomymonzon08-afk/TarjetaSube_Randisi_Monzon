using System;
using System.Collections.Generic;
using System.Text;

namespace TarjetaSube.Tests
{
    public class BoletoTests : BaseDeTests
    {
        [Test]
        public void Registrar_GuardaElBoletoConFechaTarifaYRelaciones()
        {
            var tarjeta = Tarjeta.Crear(1, "Ana");
            var colectivo = Colectivo.Crear("K");

            var boleto = Boleto.Registrar(tarjeta, colectivo, 1580);

            Contexto.Db.ChangeTracker.Clear();
            var guardado = Contexto.Db.Boletos.Find(boleto.Id);

            Assert.That(guardado, Is.Not.Null);
            Assert.That(guardado!.FechaHora, Is.EqualTo(FechaFija));
            Assert.That(guardado.Tarifa, Is.EqualTo(1580));
            Assert.That(guardado.TarjetaId, Is.EqualTo(tarjeta.Id));
            Assert.That(guardado.ColectivoId, Is.EqualTo(colectivo.Id));
        }

        [Test]
        public void Registrar_UsaLaFechaDelProveedor()
        {
            var otraFecha = new DateTime(2027, 1, 15, 8, 30, 0);
            Contexto.Fecha = new ProveedorDeFechaFalso(otraFecha);
            var tarjeta = Tarjeta.Crear(1, "Ana");
            var colectivo = Colectivo.Crear("K");

            var boleto = Boleto.Registrar(tarjeta, colectivo, 1580);

            Assert.That(boleto.FechaHora, Is.EqualTo(otraFecha));
        }
    }
}

