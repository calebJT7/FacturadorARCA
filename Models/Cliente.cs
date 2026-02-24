// aca vemos como se veria un cliente en el sistema
namespace FacturadorARCA.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string CuilCuit { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        //un cliente puede tener muchas facturas
        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}