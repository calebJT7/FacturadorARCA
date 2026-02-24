//verificar datos ingresados
using System.Text.RegularExpressions;

namespace FacturadorARCA.Utils
{
    public static class ValidacionHelper
    {
        public static bool ValidarCuilCuit(string cuilCuit)
        {
            // Debe tener 11 dígitos
            if (string.IsNullOrWhiteSpace(cuilCuit) || cuilCuit.Length != 11)
                return false;

            // Debe ser solo números
            return cuilCuit.All(char.IsDigit);
        }

        public static bool ValidarTipoFactura(string tipo)
        {
            return tipo == "A" || tipo == "B" || tipo == "C";
        }

        public static bool ValidarNumeroPositivo(decimal numero)
        {
            return numero > 0;
        }

        public static bool ValidarCantidadPositiva(int cantidad)
        {
            return cantidad > 0;
        }

        public static bool ValidarTextoNoVacio(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }
    }
}