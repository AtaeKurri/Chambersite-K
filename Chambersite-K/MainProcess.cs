using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using Chambersite_K.Views;
using Chambersite_K.Graphics;
using Chambersite_K.GameObjects;
using ImGuiNET;
using Chambersite_K.ImGUI;
using Chambersite_K.GameSettings;
using System;

namespace Chambersite_K
{
    public class MainProcess : Game
    {
        private bool _IsInitialized = false;
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private ImGuiRenderer GUIRenderer;
        public KeyboardState currentKeyboardState;
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public FrameCounter UpdateFPSCounter = new FrameCounter();
        public FrameCounter DrawFPSCounter = new FrameCounter();

        #region Resources and Objects
        public Settings Settings { get; set; } = new Settings();

        public List<Resource> GlobalResource { get; set; } = new List<Resource>();
        public GameObjectPool GlobalObjectPool { get; set; } = new GameObjectPool(false);
        internal IView? LoadingScreen { get; set; }
        internal List<IView> ActiveViews { get; set; } = new List<IView>();
        private HashSet<Guid> UsedGuids = new HashSet<Guid>();
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
            UpdateFPSCounter.Update(gameTime);
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyPressedOnce(ToggleImGUI))
                IsImGuiActive = !IsImGuiActive;

            foreach (IView view in ActiveViews)
                if (view.ViewStatus == ViewStatus.Active) view.Frame();
            foreach (GameObject gameObject in GlobalObjectPool) // Only do Frame() on those object, DON'T render them.
                gameObject.Frame();

            currentKeyboardState.UpdatePreviousKeyboardState();
            base.Update(gameTime);
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
            _spriteBatch.Begin(transformMatrix: Settings.GetViewportScale());

            RenderViewsByType();

            _spriteBatch.End();
            base.Draw(gameTime);

            DrawImGui(gameTime);
        }

        private void RenderViewsByType()
        {
            ViewType[] types = (ViewType[])Enum.GetValues(typeof(ViewType));
            foreach (ViewType type in types)
            {
                foreach (IView view in ActiveViews.FindAll(x => x.vType == type))
                    if (view.ViewStatus != ViewStatus.Hidden) view.Render();
            }
        }

        private void UpdateViewport(object sender, EventArgs e)
        {
            Viewport viewport = GraphicsDevice.Viewport;
            float aspectRatio = Settings.SettingData.ViewportSize.X / Settings.SettingData.ViewportSize.Y;
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

            GenerateGuid(ref view);
            ActiveViews.Add(view);
            if (view.RenderOrder == -999_999_999)
                view.RenderOrder = ActiveViews.Count-1;
            if (_IsInitialized)
                view.Init();

            ActiveViews.Sort((x, y) => x.RenderOrder.CompareTo(y.RenderOrder));
            return view;
        }

        private void GenerateGuid(ref IView v)
        {
            Guid uuid = Guid.NewGuid();

            while (UsedGuids.Contains(uuid))
                uuid = Guid.NewGuid();

            v.Id = uuid;
            UsedGuids.Add(uuid);
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

        public KeyboardState GetKeyboardState() => currentKeyboardState;

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
    }
}
