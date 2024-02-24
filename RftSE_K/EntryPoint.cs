using Chambersite_K;
using Microsoft.Xna.Framework.Input;
using RftSE_K.Views;

namespace RftSE_K
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainProcess game = new(args);
            MainProcess.Settings.Keybinds.Add(("RackLeft", Keys.Q));
            MainProcess.Settings.Keybinds.Add(("RackRight", Keys.D));
            game.SetLoadingScreen<MainLoadingScreen>();
            game.Run();
        }
    }
}
