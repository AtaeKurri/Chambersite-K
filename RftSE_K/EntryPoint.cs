using Chambersite_K;
using RftSE_K.Views;

namespace RftSE_K
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainProcess game = new(args);
            game.SetLoadingScreen<MainLoadingScreen>();
            game.Run();
        }
    }
}
