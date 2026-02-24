//esta clase representa cada renglon o linea de una factura 
namespace FacturadorARCA.Models
{
    public class ItemFactura
    {
        public int IdItem { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
        public int IdFactura { get; set; }
        public Factura Factura { get; set; } = null!; //muchos items pertenecen auna factura
    }
}