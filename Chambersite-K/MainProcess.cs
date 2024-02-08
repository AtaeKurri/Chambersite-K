using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using Chambersite_K.Views;
using Chambersite_K.Graphics;
using Chambersite_K.GameObjects;
using System;

namespace Chambersite_K
{
    public class MainProcess : Game
    {
        private bool _IsInitialized = false;
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        public Settings Settings { get; set; } = new Settings();

        public List<Resource> GlobalResource { get; set; } = new List<Resource>();
        public GameObjectPool GlobalObjectPool { get; set; } = new GameObjectPool();
        public IView? LoadingScreen { get; set; }
        public List<IView> ActiveViews { get; set; } = new List<IView>();

        public MainProcess()
        {
            GAME = this;
            _graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            IsMouseVisible = Settings.SettingData.IsMouseVisible;
        }

        protected override void Initialize()
        {
            Settings.LoadSettings();

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

            base.Draw(gameTime);

            _spriteBatch.End();
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>.<br/>
        /// Will only call <see cref="IView.Init"/> is the game is properly Initialized.
        /// </summary>
        /// <typeparam name="T">A <see cref="IView"/> type</typeparam>
        /// <param name="viewParams">Optional constructor arguments</param>
        /// <returns>A new instance of <typeparamref name="T"/></returns>
        public IView AddView<T>(params object[] viewParams)
        {
            IView view = (IView)Activator.CreateInstance(typeof(T), args: viewParams);
            ActiveViews.Add(view);
            if (_IsInitialized)
                view.Init();
            return view;
        }
    }
}
