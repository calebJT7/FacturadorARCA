//clase logica para manejar operaciones CRUD y validaciones 
using FacturadorARCA.Data;
using FacturadorARCA.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturadorARCA.Services
{
    public class ClienteService
    {
        private readonly FacturadorContext _context;

        public ClienteService(FacturadorContext context)
        {
            _context = context;
        }

        // manejo de excepciones
        public bool CrearCliente(Cliente cliente)
        {
            try
            {
                _context.Clientes.Add(cliente);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public List<Cliente> ObtenerTodos()
        {
            return _context.Clientes.ToList();
        }


        public Cliente? ObtenerPorId(int id)
        {
            return _context.Clientes.Find(id);
        }


        public Cliente? BuscarPorRazonSocial(string razonSocial)
        {
            return _context.Clientes
                .FirstOrDefault(c => c.RazonSocial.ToLower() == razonSocial.ToLower());
        }


        public bool ActualizarCliente(Cliente cliente)
        {
            try
            {
                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool EliminarCliente(int id)
        {
            try
            {
                var cliente = _context.Clientes.Find(id);
                if (cliente == null) return false;

                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // VALIDACIONES
        public bool ExisteCuilCuit(string cuilCuit, int? idClienteActual = null)
        {
            if (idClienteActual.HasValue)
            {
                return _context.Clientes.Any(c => c.CuilCuit == cuilCuit && c.IdCliente != idClienteActual.Value);
            }
            return _context.Clientes.Any(c => c.CuilCuit == cuilCuit);
        }
    }
}