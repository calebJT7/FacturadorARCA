using FacturadorARCA.Data;
using FacturadorARCA.Models;
using FacturadorARCA.Services;
using FacturadorARCA.Utils;
using Microsoft.EntityFrameworkCore;

internal class Program
{

    private static readonly FacturadorContext _context = new FacturadorContext();
    private static readonly ClienteService _clienteService = new ClienteService(_context);
    private static readonly FacturaService _facturaService = new FacturaService(_context);

    private static void Main(string[] args)
    {

        try
        {
            _context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            InterfazHelper.MostrarMensajeError($"Error al conectar o migrar la base de datos: {ex.Message}");
            InterfazHelper.MostrarMensajeAdvertencia("Asegúrese de que SQL LocalDB (usado en FacturadorContext) esté en ejecución.");
            InterfazHelper.EsperarTecla();
            return;
        }


        MenuPrincipal();
    }

    #region MENÚ PRINCIPAL

    private static void MenuPrincipal()
    {
        bool salir = false;
        while (!salir)
        {

            InterfazHelper.LimpiarPantalla();
            InterfazHelper.MostrarTitulo("Sistema Facturador ARCA");
            Console.WriteLine("1. Gestión de Clientes");
            Console.WriteLine("2. Gestión de Facturas");
            Console.WriteLine("-----------------------");
            Console.WriteLine("0. Salir");
            Console.WriteLine();
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    MenuGestionClientes();
                    break;
                case "2":
                    MenuGestionFacturas();
                    break;
                case "0":
                    salir = true;
                    break;
                default:

                    InterfazHelper.MostrarMensajeError("Opción no válida. Intente de nuevo.");
                    InterfazHelper.EsperarTecla();
                    break;
            }
        }
    }

    #endregion

    #region GESTIÓN DE CLIENTES


    private static void MenuGestionClientes()
    {
        bool volver = false;
        while (!volver)
        {
            InterfazHelper.LimpiarPantalla();
            InterfazHelper.MostrarTitulo("Gestión de Clientes");
            Console.WriteLine("1. Dar de alta un cliente (Alta)");
            Console.WriteLine("2. Actualizar datos de un cliente (Modificación)");
            Console.WriteLine("3. Eliminar un cliente (Baja)");
            Console.WriteLine("4. Listar todos los clientes (Listado)");
            Console.WriteLine("5. Buscar cliente por Razón Social (Consulta)");
            Console.WriteLine("-----------------------");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine();
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine() ?? "";


            switch (opcion)
            {
                case "1":
                    DarAltaCliente();
                    break;
                case "2":
                    ActualizarCliente();
                    break;
                case "3":
                    EliminarCliente();
                    break;
                case "4":
                    ListarClientes();
                    break;
                case "5":
                    BuscarClientePorRazonSocial();
                    break;
                case "0":
                    volver = true;
                    break;
                default:
                    InterfazHelper.MostrarMensajeError("Opción no válida.");
                    InterfazHelper.EsperarTecla();
                    break;
            }
        }
    }

    private static void DarAltaCliente()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Alta de Cliente");

        string razonSocial;
        do
        {
            Console.Write("Razón Social: ");
            razonSocial = Console.ReadLine() ?? "";
            //Validación y manejo de errores
        } while (!ValidacionHelper.ValidarTextoNoVacio(razonSocial));

        string domicilio;
        do
        {
            Console.Write("Domicilio: ");
            domicilio = Console.ReadLine() ?? "";
        } while (!ValidacionHelper.ValidarTextoNoVacio(domicilio));

        string cuilCuit;
        bool cuilValido;
        do
        {
            Console.Write("CUIL/CUIT (11 dígitos, sin guiones): ");
            cuilCuit = Console.ReadLine() ?? "";
            cuilValido = ValidacionHelper.ValidarCuilCuit(cuilCuit);

            if (!cuilValido)
            {
                InterfazHelper.MostrarMensajeError("Formato de CUIL/CUIT incorrecto. Debe tener 11 dígitos numéricos.");
            }

            else if (_clienteService.ExisteCuilCuit(cuilCuit))
            {
                InterfazHelper.MostrarMensajeError("El CUIL/CUIT ingresado ya existe en la base de datos.");
                cuilValido = false; // Forzar a pedir de nuevo
            }
        } while (!cuilValido);

        Cliente nuevoCliente = new Cliente
        {
            RazonSocial = razonSocial,
            Domicilio = domicilio,
            CuilCuit = cuilCuit
        };

        if (_clienteService.CrearCliente(nuevoCliente))
        {
            InterfazHelper.MostrarMensajeExito("Cliente creado exitosamente.");
        }
        else
        {
            InterfazHelper.MostrarMensajeError("Ocurrió un error al guardar el cliente.");
        }

        InterfazHelper.EsperarTecla();
    }

    private static void ActualizarCliente()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Actualizar Cliente");

        Console.Write("Ingrese el ID del cliente a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int idCliente))
        {
            InterfazHelper.MostrarMensajeError("ID no válido.");
            InterfazHelper.EsperarTecla();
            return;
        }

        var cliente = _clienteService.ObtenerPorId(idCliente);
        if (cliente == null)
        {
            InterfazHelper.MostrarMensajeError("Cliente no encontrado.");
            InterfazHelper.EsperarTecla();
            return;
        }

        // Pedir nuevos datos, mostrando los actuales
        string razonSocial;
        do
        {
            Console.Write($"Razón Social ({cliente.RazonSocial}): ");
            razonSocial = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(razonSocial)) razonSocial = cliente.RazonSocial;
        } while (!ValidacionHelper.ValidarTextoNoVacio(razonSocial));

        string domicilio;
        do
        {
            Console.Write($"Domicilio ({cliente.Domicilio}): ");
            domicilio = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(domicilio)) domicilio = cliente.Domicilio;
        } while (!ValidacionHelper.ValidarTextoNoVacio(domicilio));

        string cuilCuit;
        bool cuilValido;
        do
        {
            Console.Write($"CUIL/CUIT ({cliente.CuilCuit}): ");
            cuilCuit = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(cuilCuit)) cuilCuit = cliente.CuilCuit;

            cuilValido = ValidacionHelper.ValidarCuilCuit(cuilCuit);

            if (!cuilValido)
            {
                InterfazHelper.MostrarMensajeError("Formato de CUIL/CUIT incorrecto.");
            }
            // Validacion de que no exista OTRO cliente con ese CUIT
            else if (_clienteService.ExisteCuilCuit(cuilCuit, cliente.IdCliente))
            {
                InterfazHelper.MostrarMensajeError("El CUIL/CUIT ingresado ya pertenece a otro cliente.");
                cuilValido = false;
            }
        } while (!cuilValido);

        cliente.RazonSocial = razonSocial;
        cliente.Domicilio = domicilio;
        cliente.CuilCuit = cuilCuit;

        if (_clienteService.ActualizarCliente(cliente))
        {
            InterfazHelper.MostrarMensajeExito("Cliente actualizado exitosamente.");
        }
        else
        {
            InterfazHelper.MostrarMensajeError("Ocurrió un error al actualizar el cliente.");
        }

        InterfazHelper.EsperarTecla();
    }

    private static void EliminarCliente()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Eliminar Cliente");

        Console.Write("Ingrese el ID del cliente a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int idCliente))
        {
            InterfazHelper.MostrarMensajeError("ID no válido.");
            InterfazHelper.EsperarTecla();
            return;
        }

        var cliente = _clienteService.ObtenerPorId(idCliente);
        if (cliente == null)
        {
            InterfazHelper.MostrarMensajeError("Cliente no encontrado.");
            InterfazHelper.EsperarTecla();
            return;
        }

        // Aplicación de POO y relaciones

        if (_context.Facturas.Any(f => f.IdCliente == idCliente))
        {
            InterfazHelper.MostrarMensajeError("No se puede eliminar el cliente porque tiene facturas asociadas.");
            InterfazHelper.MostrarMensajeAdvertencia("Esta validación previene errores de base de datos.");
            InterfazHelper.EsperarTecla();
            return;
        }

        if (InterfazHelper.ConfirmarAccion($"¿Está seguro de que desea eliminar a '{cliente.RazonSocial}' (ID: {cliente.IdCliente})?"))
        {
            if (_clienteService.EliminarCliente(idCliente))
            {
                InterfazHelper.MostrarMensajeExito("Cliente eliminado exitosamente.");
            }
            else
            {
                InterfazHelper.MostrarMensajeError("Ocurrió un error al eliminar el cliente.");
            }
        }
        else
        {
            InterfazHelper.MostrarMensajeAdvertencia("Eliminación cancelada.");
        }

        InterfazHelper.EsperarTecla();
    }

    private static void ListarClientes()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Listado de Clientes");

        var clientes = _clienteService.ObtenerTodos();

        if (clientes.Count == 0)
        {
            InterfazHelper.MostrarMensajeAdvertencia("No hay clientes registrados.");
        }
        else
        {
            Console.WriteLine($"{"ID",-5} | {"Razón Social",-40} | {"CUIL/CUIT",-15} | {"Domicilio",-40}");
            Console.WriteLine(new string('-', 105));
            foreach (var cliente in clientes)
            {
                Console.WriteLine($"{cliente.IdCliente,-5} | {cliente.RazonSocial,-40} | {cliente.CuilCuit,-15} | {cliente.Domicilio,-40}");
            }
        }

        InterfazHelper.EsperarTecla();
    }

    private static void BuscarClientePorRazonSocial()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Buscar Cliente por Razón Social");

        Console.Write("Ingrese la Razón Social exacta a buscar: ");
        string busqueda = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(busqueda))
        {
            InterfazHelper.MostrarMensajeError("Debe ingresar un término de búsqueda.");
            InterfazHelper.EsperarTecla();
            return;
        }


        var cliente = _clienteService.BuscarPorRazonSocial(busqueda);

        if (cliente == null)
        {
            InterfazHelper.MostrarMensajeError("No se encontró ningún cliente con esa Razón Social exacta.");
        }
        else
        {
            Console.WriteLine("\nCliente encontrado:");
            Console.WriteLine(new string('-', 30));
            Console.WriteLine($"ID: {cliente.IdCliente}");
            Console.WriteLine($"Razón Social: {cliente.RazonSocial}");
            Console.WriteLine($"CUIL/CUIT: {cliente.CuilCuit}");
            Console.WriteLine($"Domicilio: {cliente.Domicilio}");
        }

        InterfazHelper.EsperarTecla();
    }


    #endregion

    #region GESTIÓN DE FACTURAS

    // CRUD (Alta, Baja, Modificación, Consulta y Listado) de las siguientes entidades: Factura
    // Nota: El "Alta" es "Emitir". "Consulta" es "Ver Detalle". "Listado" es "Listar".
    private static void MenuGestionFacturas()
    {
        bool volver = false;
        while (!volver)
        {
            InterfazHelper.LimpiarPantalla();
            InterfazHelper.MostrarTitulo("Gestión de Facturas");
            Console.WriteLine("1. Emitir nueva factura (Alta)");
            Console.WriteLine("2. Listar todas las facturas (Listado)");
            Console.WriteLine("3. Ver detalle de una factura (Consulta)");
            Console.WriteLine("-----------------------");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine();
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    EmitirFactura();
                    break;
                case "2":
                    ListarFacturas();
                    break;
                case "3":
                    VerDetalleFactura();
                    break;
                case "0":
                    volver = true;
                    break;
                default:
                    InterfazHelper.MostrarMensajeError("Opción no válida.");
                    InterfazHelper.EsperarTecla();
                    break;
            }
        }
    }

    private static void EmitirFactura()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Emitir Nueva Factura");

        //para poder emitir una Factura a un Cliente, el mismo debe estar previamente registrado
        Cliente? cliente = SeleccionarClienteInteractivo();
        if (cliente == null)
        {
            InterfazHelper.MostrarMensajeAdvertencia("Emisión cancelada. No se seleccionó o encontró un cliente válido.");
            InterfazHelper.EsperarTecla();
            return;
        }

        InterfazHelper.MostrarMensajeExito($"Cliente seleccionado: {cliente.RazonSocial} (CUIT: {cliente.CuilCuit})");
        Console.WriteLine($"Domicilio: {cliente.Domicilio}");
        Console.WriteLine();

        //solicitar al usuario el tipo de factura que desea emitir (A, B o C)
        string tipo;
        do
        {
            Console.Write("Ingrese Tipo de Factura (A, B, C): ");
            tipo = (Console.ReadLine() ?? "").ToUpper();
            if (!ValidacionHelper.ValidarTipoFactura(tipo))
            {
                InterfazHelper.MostrarMensajeError("Tipo no válido. Debe ser A, B o C.");
            }
        } while (!ValidacionHelper.ValidarTipoFactura(tipo));

        Factura nuevaFactura = new Factura
        {
            IdCliente = cliente.IdCliente,
            Fecha = DateTime.Now,
            Tipo = tipo,
            NumeroFactura = _facturaService.GenerarNumeroFactura(tipo),
            Items = new List<ItemFactura>()
        };
        bool agregarMasItems = true;
        while (agregarMasItems)
        {
            Console.WriteLine("\n--- Nuevo Item ---");

            string descripcion;
            do
            {
                Console.Write("Descripción: ");
                descripcion = Console.ReadLine() ?? "";
            } while (!ValidacionHelper.ValidarTextoNoVacio(descripcion));

            int cantidad;
            do
            {
                Console.Write("Cantidad: ");

            } while (!int.TryParse(Console.ReadLine(), out cantidad) || !ValidacionHelper.ValidarCantidadPositiva(cantidad));

            decimal importe; // Importe unitario
            do
            {
                Console.Write("Importe Unitario (Ej: 150.50): ");
            } while (!decimal.TryParse(Console.ReadLine(), out importe) || !ValidacionHelper.ValidarNumeroPositivo(importe));

            ItemFactura item = new ItemFactura
            {
                Descripcion = descripcion,
                Cantidad = cantidad,
                Importe = importe,

            };

            nuevaFactura.Items.Add(item);
            InterfazHelper.MostrarMensajeExito($"Item agregado Subtotal:{(item.Cantidad * item.Importe)}");

            agregarMasItems = InterfazHelper.ConfirmarAccion("¿Desea agregar otro item a la factura?");
        }

        if (nuevaFactura.Items.Count == 0)
        {
            InterfazHelper.MostrarMensajeAdvertencia("Emisión cancelada (la factura no tiene items).");
            InterfazHelper.EsperarTecla();
            return;
        }


        if (_facturaService.EmitirFactura(nuevaFactura))
        {

            InterfazHelper.MostrarMensajeExito($"Factura {nuevaFactura.NumeroFactura} emitida exitosamente por un total de {nuevaFactura.Items.Sum(i => i.Cantidad * i.Importe):C}.");
        }
        else
        {
            InterfazHelper.MostrarMensajeError("Ocurrió un error al guardar la factura.");
        }

        InterfazHelper.EsperarTecla();
    }
    private static Cliente? SeleccionarClienteInteractivo()
    {
        Console.Write("Ingrese Razón Social del cliente a facturar: ");
        string busqueda = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(busqueda)) return null;

        var cliente = _clienteService.BuscarPorRazonSocial(busqueda);

        return cliente;
    }

    private static void ListarFacturas()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Listado de Facturas");

        var facturas = _facturaService.ObtenerTodas();

        if (facturas.Count == 0)
        {
            InterfazHelper.MostrarMensajeAdvertencia("No hay facturas emitidas.");
        }
        else
        {
            // Mostramos datos del cliente relacionado (f.Cliente.RazonSocial)
            Console.WriteLine($"{"ID",-5} | {"Número",-12} | {"Tipo",-5} | {"Fecha",-20} | {"Cliente",-30} | {"Total",-15}");
            Console.WriteLine(new string('-', 90));
            foreach (var f in facturas)
            {
                string nombreCliente = f.Cliente?.RazonSocial ?? "N/A";
                decimal total = 0m;
                if (f.Items != null)
                {
                    foreach (var it in f.Items)
                    {
                        total += it.Cantidad * it.Importe;
                    }
                }
                Console.WriteLine($"{f.IdFactura,-5} | {f.NumeroFactura,-12} | {f.Tipo,-5} | {f.Fecha,-20:dd/MM/yyyy HH:mm} | {nombreCliente,-30} | {total,-15:C}");
            }
        }

        InterfazHelper.EsperarTecla();
    }

    private static void VerDetalleFactura()
    {
        InterfazHelper.LimpiarPantalla();
        InterfazHelper.MostrarTitulo("Detalle de Factura (Consulta)");

        Console.Write("Ingrese el ID de la factura a consultar: ");
        if (!int.TryParse(Console.ReadLine(), out int idFactura))
        {
            InterfazHelper.MostrarMensajeError("ID no válido.");
            InterfazHelper.EsperarTecla();
            return;
        }
        var factura = _facturaService.ObtenerPorId(idFactura);

        if (factura == null)
        {
            InterfazHelper.MostrarMensajeError("Factura no encontrada.");
            InterfazHelper.EsperarTecla();
            return;
        }
        // muestra detalle completo de la factura
        Console.WriteLine($"Factura: {factura.NumeroFactura} (Tipo {factura.Tipo})");
        Console.WriteLine($"Fecha: {factura.Fecha:dd/MM/yyyy}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("Cliente:");
        Console.WriteLine($"  Razón Social: {factura.Cliente.RazonSocial}");
        Console.WriteLine($"  CUIT: {factura.Cliente.CuilCuit}");
        Console.WriteLine($"  Domicilio: {factura.Cliente.Domicilio}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("Items:");
        Console.WriteLine("  {0,-5} | {1,-30} | {2,-15} | {3,-15}", "Cant", "Descripción", "P. Unitario", "Subtotal");
        Console.WriteLine(new string('-', 68));

        foreach (var item in factura.Items)
        {
            Console.WriteLine($"  {item.Cantidad,-5} | {item.Descripcion,-30} | {item.Importe,-15:C} | {(item.Cantidad * item.Importe),-15:C}");
        }

        Console.WriteLine(new string('-', 68));
        Console.WriteLine($"TOTAL FACTURA: {factura.Items.Sum(i => i.Cantidad * i.Importe),49:C}");
        Console.WriteLine(new string('=', 60));

        InterfazHelper.EsperarTecla();
    }

    #endregion
}