using Chambersite_K;
using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using MainProcess game = new MainProcess();
            game.AddView<MainMenu>();
            game.Run();
        }
    }

    [ViewType(ViewType.Menu)]
    public class MainMenu : View
    {
        public override void Init()
        {
            Resource.LoadGlobalResource<Texture2D>("Tial", "Tial.png");
            CreateGameObject<TestObject>();
        }
    }

    public class TestObject : GameObject
    {
        public override void Init()
        {
            Image = Resource.FindResource<Texture2D>("Tial", (MainMenu)Parent);
            base.Init();
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
