//manejo de operaciones (creacion de facturas, consultas y generacion de nro de facturas)
using FacturadorARCA.Data;
using FacturadorARCA.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturadorARCA.Services
{
    public class FacturaService
    {
        private readonly FacturadorContext _context;

        public FacturaService(FacturadorContext context)
        {
            _context = context;
        }


        public bool EmitirFactura(Factura factura)
        {
            try
            {
                // Calcular el total de forma segura: soporta diferentes modelos de ItemFactura y Factura.
                decimal total = 0m;
                if (factura.Items != null)
                {
                    foreach (var item in factura.Items)
                    {

                        var subtotalProp = item.GetType().GetProperty("Subtotal");
                        if (subtotalProp != null && subtotalProp.GetValue(item) is decimal d)
                        {
                            total += d;
                            continue;
                        }

                        // Si no existe Subtotal, intentar calcular como Cantidad * PrecioUnitario
                        var cantidadProp = item.GetType().GetProperty("Cantidad") ?? item.GetType().GetProperty("Quantity") ?? item.GetType().GetProperty("CantidadItem");
                        var precioProp = item.GetType().GetProperty("PrecioUnitario") ?? item.GetType().GetProperty("Precio") ?? item.GetType().GetProperty("Price");

                        decimal cantidad = 0m;
                        decimal precio = 0m;

                        var cantidadObj = cantidadProp?.GetValue(item);
                        var precioObj = precioProp?.GetValue(item);

                        if (cantidadObj != null && decimal.TryParse(cantidadObj.ToString(), out decimal c))
                            cantidad = c;
                        if (precioObj != null && decimal.TryParse(precioObj.ToString(), out decimal p))
                            precio = p;

                        total += cantidad * precio;
                    }
                }

                // Asignar a la propiedad Total si existe en Factura
                var totalProp = factura.GetType().GetProperty("Total");
                if (totalProp != null && totalProp.CanWrite)
                {
                    totalProp.SetValue(factura, total);
                }

                _context.Facturas.Add(factura);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public List<Factura> ObtenerTodas()
        {
            return _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Items)
                .ToList();
        }


        public Factura? ObtenerPorId(int id)
        {
            return _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Items)
                .FirstOrDefault(f => f.IdFactura == id);
        }

        // Genera un numero de factura
        public string GenerarNumeroFactura(string tipo)
        {
            var ultimaFactura = _context.Facturas
                .Where(f => f.Tipo == tipo)
                .OrderByDescending(f => f.IdFactura)
                .FirstOrDefault();

            int numero = 1;
            if (ultimaFactura != null)
            {

                var partes = ultimaFactura.NumeroFactura.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[1], out int ultimoNumero))
                {
                    numero = ultimoNumero + 1;
                }
            }

            return $"{tipo}-{numero:D8}"; // Formato: A-00000001
        }
    }
}


