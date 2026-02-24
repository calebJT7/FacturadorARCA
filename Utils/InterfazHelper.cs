namespace FacturadorARCA.Utils
{
    public static class InterfazHelper
    {
        public static void LimpiarPantalla()
        {
            Console.Clear();
        }

        public static void MostrarTitulo(string titulo)
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"   {titulo}");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();
        }

        public static void MostrarMensajeExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ {mensaje}");
            Console.ResetColor();
        }

        public static void MostrarMensajeError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n✗ {mensaje}");
            Console.ResetColor();
        }

        public static void MostrarMensajeAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n⚠ {mensaje}");
            Console.ResetColor();
        }

        public static void EsperarTecla()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public static bool ConfirmarAccion(string mensaje)
        {
            Console.Write($"\n{mensaje} (S/N): ");
            var respuesta = Console.ReadLine()?.ToUpper();
            return respuesta == "S" || respuesta == "SI";
        }
    }
}