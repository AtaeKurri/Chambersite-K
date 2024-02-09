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
            using MainProcess game = new MainProcess(true);
            game.AddView<MainMenu>();
            game.AddView<GameUI>();
            game.AddView<TestStage>();
            game.Run();
        }
    }

    [ViewName("Main Menu")]
    [ViewType(ViewType.Menu)]
    public class MainMenu : View
    {
        public override void Init()
        {
            Resource.LoadGlobalResource<Texture2D>("Tial", "Tial.png");
            CreateGameObject<TestObject>();
        }
    }

    [ViewName("UI")]
    [ViewType(ViewType.Interface)]
    public class GameUI : View
    {

    }

    [ViewName("Stage Test")]
    [ViewType(ViewType.Stage)]
    public class TestStage : View
    {

    }

    public class TestObject : GameObject
    {
        public override void Init()
        {
            Scale = new Vector2(0.3f, 0.3f);
            Velocity = 0f;
            Image = Resource.FindResource<Texture2D>("Tial", (MainMenu)Parent);
            base.Init();
        }
    }
}
