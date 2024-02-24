using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Chambersite_K.Views;
using Chambersite_K.Graphics;
using Chambersite_K.GameObjects;
using ImGuiNET;
using Chambersite_K.ImGUI;
using Chambersite_K.GameSettings;
using CommandLine;

namespace Chambersite_K
{
    public class MainProcess : Game, IResourceHolder
    {
        internal bool _IsInitialized = false;
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private ImGuiRenderer GUIRenderer;
        public KeyboardState currentKeyboardState;
        public FrameCounter UpdateFPSCounter = new();
        public FrameCounter DrawFPSCounter = new();
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #region Resources and Objects

        public static Settings Settings { get; set; } = new Settings();

        public List<IResource> ResourcePool { get; set; } = new List<IResource>();
        public GameObjectPool ObjectPool { get; set; }
        internal LoadingScreen? LoadingScreen { get; set; }
        internal ViewPool ActiveViews { get; set; }

        public List<IResource> GetGlobalResources() => ResourcePool;

        #endregion
        #region ImGui Windows

        public bool AllowImGui = false;
        private bool IsImGuiActive = true;

        /// <summary>
        /// Change this this to determine which key toggles the ImGUI display.
        /// </summary>
        public Keys ToggleImGUI = Keys.F3;
        private GUIViewAndObjectList GUI_ViewAndObjectList = new GUIViewAndObjectList();
        private GUIFrameStats GUI_FrameStats = new GUIFrameStats();

        #endregion

        public MainProcess(string[] args)
        {
            if (GAME != null)
                throw new ApplicationException("A MainProcess instance already exists.");
            GAME = this;
            _graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            ObjectPool = new GameObjectPool(this);
            ActiveViews = new ViewPool(this);
            IsMouseVisible = Settings.IsMouseVisible;

            DoCmds(args);
        }

        private void DoCmds(string[] args)
        {
            Parser parser = new(with => with.EnableDashDash = true);
            ParserResult<CmdOptions> cmds = parser.ParseArguments<CmdOptions>(args);

            AllowImGui = cmds.Value.UseImGui;
            if (cmds.Value.OpenConsole) ConsoleHelper.ShowConsole();

            Logger.Info($"Launch arguments: {string.Join(" ", args)}");
        }

        #region Game Loop

        protected override void Initialize()
        {
            Logger.Debug("MainProcess initialization start.");
            Settings.LoadSettings();
            GUIRenderer = new ImGuiRenderer(this);
            GUIRenderer.RebuildFontAtlas();

            // TODO: Si y'a un LoadingScreen mis au lancement du jeu, lancer la View correspondante, sinon lancer le menu quand toutes les
            // globalResources ont été chargés.

            Window.ClientSizeChanged += UpdateViewport;

            base.Initialize();
            _IsInitialized = true;
            Logger.Debug("MainProcess was initialized correctly.");
            LoadingScreen.Initialize();
            /*foreach (IView view in ActiveViews)
            {
                if (!view.WasInitialized)
                    view.Init();
            }*/
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected void BeforeUpdate(GameTime gameTime)
        {
            UpdateFPSCounter.Update(gameTime);
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyPressedOnce(ToggleImGUI))
                IsImGuiActive = !IsImGuiActive;

            LoadingScreen?.BeforeUpdate();
            ActiveViews.BeforeUpdate();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!_IsInitialized)
                return;
            BeforeUpdate(gameTime);

            LoadingScreen?.Update();
            ActiveViews.Update();
            ObjectPool.Update();

            base.Update(gameTime);

            AfterUpdate(gameTime);
        }

        protected void AfterUpdate(GameTime gameTime)
        {
            LoadingScreen?.AfterUpdate();
            ActiveViews.AfterUpdate();

            currentKeyboardState.UpdatePreviousKeyboardState();
        }

        /// <summary>
        /// This methods skips the rendering of global objects, because they're meant to be used as information holder.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            if (!_IsInitialized)
                return;
            DrawFPSCounter.Update(gameTime);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                Settings.SampleState,
                null,
                null,
                null,
                Settings.GetViewportScale());

            LoadingScreen?.Draw();
            ActiveViews.Draw();

            _spriteBatch.End();
            base.Draw(gameTime);

            DrawImGui(gameTime);
        }

        public static KeyboardState GetKeyboardState() => Keyboard.GetState();

        #endregion
        #region Views and GameObjects

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
            IView view = ActiveViews.AddView<T>(viewParams);
            return view;
        }

        public LoadingScreen SetLoadingScreen<T>()
        {
            if (LoadingScreen != null)
            {
                throw new ApplicationException("A Loading Screen already exists, cannot create another one");
            }
            LoadingScreen loader = (LoadingScreen)Activator.CreateInstance(typeof(T));
            loader.Parent = this;
            this.LoadingScreen = loader;
            return loader;
        }

        private void UpdateViewport(object sender, EventArgs e)
        {
            Viewport viewport = GraphicsDevice.Viewport;
            float aspectRatio = Settings.ViewportSize.X / Settings.ViewportSize.Y;
            int width = viewport.Width;
            int height = (int)(width / aspectRatio + 0.5f);

            if (height > viewport.Height)
            {
                height = viewport.Height;
                width = (int)(height * aspectRatio + 0.5f);
            }

            int x = (viewport.Width / 2) - (width / 2);
            int y = (viewport.Height / 2) - (height / 2);

            GraphicsDevice.Viewport = new Viewport(x, y, width, height);
        }

        #endregion
        #region ImGui

        private void DrawImGui(GameTime gameTime)
        {
            if (!AllowImGui || !IsImGuiActive) return;
            GUIRenderer.BeforeLayout(gameTime);

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Game"))
                {
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem("Frame Statistics", null, GUI_FrameStats.ShowWindow)) GUI_FrameStats.ShowWindow = !GUI_FrameStats.ShowWindow;
                    if (ImGui.MenuItem("Object Instances", null, GUI_ViewAndObjectList.ShowWindow)) GUI_ViewAndObjectList.ShowWindow = !GUI_ViewAndObjectList.ShowWindow;
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            GUI_ViewAndObjectList.Draw();
            GUI_FrameStats.Draw(DrawFPSCounter, UpdateFPSCounter);

            GUIRenderer.AfterLayout();
        }

        #endregion
    }
}
