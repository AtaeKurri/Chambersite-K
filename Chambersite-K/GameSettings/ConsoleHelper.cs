using System.Runtime.InteropServices;

namespace Chambersite_K.GameSettings
{
    public static class ConsoleHelper
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public static void ShowConsole()
        {
            AllocConsole();
        }

        public static void HideConsole()
        {
            FreeConsole();
        }
    }
}
