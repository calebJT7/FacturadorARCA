namespace FacturadorARCA.Models
{
    public class Factura
    {
        //enlaza la factura con el cliente
        public int IdFactura { get; set; }
        //en esta linea se almacena el numero de la factura
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        //muchas facturas pertenecen a un cliente

        public Cliente Cliente { get; set; } = null!;
        public ICollection<ItemFactura> Items { get; set; } = new List<ItemFactura>();
    }
}