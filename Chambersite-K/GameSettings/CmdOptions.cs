using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameSettings
{
    public class CmdOptions(bool openConsole, bool useImGui)
    {
        private readonly bool _OpenConsole = openConsole;
        private readonly bool _UseImGui = useImGui;

        [Option("console", Required = false, HelpText = "Open the game with the debug console.")]
        public bool OpenConsole { get { return _OpenConsole; } }

        [Option("imgui", Required = false, HelpText = "Allow the use of the ImGui interface for debugging.")]
        public bool UseImGui { get { return _UseImGui; } }
    }
}
