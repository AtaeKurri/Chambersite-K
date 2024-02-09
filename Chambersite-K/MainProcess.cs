using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using Chambersite_K.Views;
using Chambersite_K.Graphics;
using Chambersite_K.GameObjects;
using ImGuiNET;
using Chambersite_K.ImGUI;

namespace Chambersite_K
{
    public class MainProcess : Game
    {
        private bool _IsInitialized = false;
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private ImGuiRenderer GUIRenderer;

        public Settings Settings { get; set; } = new Settings();

        public List<Resource> GlobalResource { get; set; } = new List<Resource>();
        public GameObjectPool GlobalObjectPool { get; set; } = new GameObjectPool();
        internal IView? LoadingScreen { get; set; }
        internal List<IView> ActiveViews { get; set; } = new List<IView>();
        private long nextViewId { get; set; } = 0;

        public bool AllowImGui = false;
        private bool IsImGuiActive = true;
        private int ImGuiSelectedViewIndex = 0;

        public MainProcess(bool allowImGui)
        {
            if (GAME != null)
                throw new ApplicationException("A MainProcess instance already exists.");
            GAME = this;
            _graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            IsMouseVisible = Settings.SettingData.IsMouseVisible;
            AllowImGui = allowImGui;
        }

        protected override void Initialize()
        {
            Settings.LoadSettings();
            GUIRenderer = new ImGuiRenderer(this);
            GUIRenderer.RebuildFontAtlas();

            // TODO: Si y'a un LoadingScreen mis au lancement du jeu, lancer la View correspondante, sinon lancer le menu quand toutes les
            // globalResources ont été chargés.

            base.Initialize();
            _IsInitialized = true;
            foreach (IView view in ActiveViews)
            {
                if (!view.WasInitialized)
                    view.Init();
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!_IsInitialized)
                return;
            foreach (IView view in ActiveViews)
                view.Frame();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_IsInitialized)
                return;
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            //_spriteBatch.Draw(testImg, new Vector2(0, 0), Color.White);
            foreach (IView view in ActiveViews)
                view.Render();

            _spriteBatch.End();

            base.Draw(gameTime);

            if (!AllowImGui) return;

            GUIRenderer.BeforeLayout(gameTime);

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 500), ImGuiCond.Always);
            ImGui.Begin("Object Inspector");

            ImGui.Combo("Views", ref ImGuiSelectedViewIndex, GetViewsNameArray(ActiveViews), ActiveViews.Count);
            if (ImGuiSelectedViewIndex >= 0 && ImGuiSelectedViewIndex < ActiveViews.Count)
            {
                ImGui.Text("Children: ");
                foreach (GameObject obj in ActiveViews[ImGuiSelectedViewIndex].Children)
                {
                    ImGui.BulletText($"{obj} (Id: {obj.Id} ; Status: {obj.Status})");
                }
            }

            ImGui.End();

            GUIRenderer.AfterLayout();
        }

        public string[] GetViewsNameArray(List<IView> views)
        {
            string[] viewNames = new string[views.Count];
            for (int i = 0; i < views.Count; i++)
            {
                viewNames[i] = $"{views[i].InternalName} (Id: {views[i].Id} ; Type: {views[i].vType})";
            }
            return viewNames;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>.<br/>
        /// Will only call <see cref="IView.Init"/> is the game is properly Initialized.
        /// </summary>
        /// <typeparam name="T">A <see cref="IView"/> type</typeparam>
        /// <param name="viewParams">Optional constructor arguments</param>
        /// <returns>A new instance of <typeparamref name="T"/></returns>
        /// <exception cref="InvalidViewOperationException">Will be thrown if another stage already exists or if you try to add a Loading Screen.</exception>
        public IView AddView<T>(params object[] viewParams)
        {
            IView view = (IView)Activator.CreateInstance(typeof(T), args: viewParams);
            if (view.vType == ViewType.Stage && ActiveViews.Any(x => x.vType == ViewType.Stage))
                throw new InvalidViewOperationException("Multiple stages cannot co-exist. If you wish to switch to another stage, please use the correct method.");
            else if (view.vType == ViewType.Background && ActiveViews.Any(x => x.vType == ViewType.Background))
                throw new InvalidViewOperationException("Multiple Backgrounds cannot co-exist.");
            else if (view.vType == ViewType.LoadingScreen)
                throw new InvalidViewOperationException("You cannot add a Loading Screen into the Active Views.");

            view.Id = nextViewId;
            ActiveViews.Add(view);
            if (_IsInitialized)
                view.Init();
            nextViewId++;
            return view;
        }

        /// <summary>
        /// Attepts to create a new Stage and delete the existing one. Will fail if the view type provided is not of <see cref="ViewType.Stage"/>.
        /// </summary>
        /// <typeparam name="T">A <see cref="IView"/> type</typeparam>
        /// <param name="viewParams">Optional constructor arguments</param>
        /// <returns>A new instance of <typeparamref name="T"/></returns>
        /// <exception cref="InvalidViewOperationException">Will be thrown if the view type is not a Stage View.</exception>
        public IView SwitchToStage<T>(params object[] viewParams)
        {
            IView view = (IView)Activator.CreateInstance(typeof(T), args: viewParams);
            if (view.vType != ViewType.Stage)
                throw new InvalidViewOperationException("You cannot switch to a non-stage view.");

            ActiveViews.RemoveAll(x => x.vType == ViewType.Stage);
            ActiveViews.Add(view);
            if (_IsInitialized)
                view.Init();
            return view;
        }
    }
}
