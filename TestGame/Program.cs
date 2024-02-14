using Boleite.Players;
using Chambersite_K;
using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Chambersite_K.GameSettings;
using Chambersite_K.Views;
using Chambersite_K.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using MainProcess game = new MainProcess(true);
            game.AddView<MainStage>();
            game.AddView<GameUI>();
            game.AddView<TestBackground>();
            game.Run();

            /*
             * Let people define their configs.
             * People must be allowed to define an enumerator for the GameObject groups
             * and set the collision checking routing themselves.
             */
        }
    }

    [InternalName("Main Stage")]
    [ViewType(ViewType.Stage)]
    public class MainStage : View
    {
        public override void Init()
        {
            LoadLocalResource<Texture2D>("Tial", "Assets/Tial.png");
            //Resource.LoadGlobalResource<Texture2D>("Tial", "Tial.png");
            CreateGameObject<TestObject>();
            CreateGameObject<Player>();
            base.Init();
        }
    }

    [InternalName("UI")]
    [ViewType(ViewType.Interface)]
    public class GameUI : View
    {
        public override void Init()
        {
            LoadLocalResource<Texture2D>("UI", "Assets/ui_bg.png");
            CreateGameObject<UIBG>();
            base.Init();
        }
    }

    [InternalName("Main Background")]
    [ViewType(ViewType.Background)]
    public class TestBackground : View
    {
        public World3D World { get; set; } = new World3D();

        public override void Init()
        {
            base.Init();
        }
    }

    [InternalName("Tial Test")]
    public class TestObject : GameObject
    {
        public int TestProperty { get; set; } = 0;
        public override void Init()
        {
            Position = new Vector2(MainProcess.Settings.SettingData.ViewportSize.X / 2, MainProcess.Settings.SettingData.ViewportSize.Y / 2);
            Scale = new Vector2(0.3f, 0.3f);
            RotationDegrees = 90f;
            Velocity = 0f;
            Image = "Tial";
            base.Init();
        }

        public override void Frame()
        {
            //RotationDegrees += 1.0f;
            base.Frame();
        }
    }

    [InternalName("Main Interface")]
    public class UIBG : GameObject
    {
        public override void Init()
        {
            Image = "UI";
            Position = new Vector2(MainProcess.Settings.SettingData.ViewportSize.X / 2, MainProcess.Settings.SettingData.ViewportSize.Y / 2);
            base.Init();
        }
    }
}
