﻿using Chambersite_K.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public delegate void ResourcesLoadedEventHandler();

    /// <summary>
    /// Base class for handling loading global resources at the game's launch.<br/>
    /// Use <see cref="AddResourceLoader{GlobalResourceLoader}"/> to add resource loaders.
    /// </summary>
    public abstract class LoadingScreen : IResourceHolder
    {
        public MainProcess Parent { get; set; }
        public List<GlobalResourceLoader> ResourceLoaders { get; set; } = new List<GlobalResourceLoader>();
        public bool AreResourceLoaded { get; set; } = false;
        public List<IResource> ResourcePool { get; set; } = new List<IResource>();
        public int TotalResourcesLoadedNum { get; set; } = 0;

        public event ResourcesLoadedEventHandler ResourcesLoaded;

        public LoadingScreen()
        {
            ResourcesLoaded += AfterLoading;
        }

        public virtual void Init()
        {
            Task.Run(LoadContent);
        }

        public virtual void Frame(GameTime gameTime)
        {

        }

        public virtual void Render()
        {

        }

        public void AddResourceLoader<GlobalResourceLoader>()
        {
            Graphics.GlobalResourceLoader loader = (Graphics.GlobalResourceLoader)Activator.CreateInstance(typeof(GlobalResourceLoader));
            ResourceLoaders.Add(loader);
        }

        /// <summary>
        /// Raised by the <see cref="ResourcesLoadedEventHandler"/> event after all resources were successfully loaded.<br/>
        /// In base behaviour, this method calls <see cref="IView.Init"/> on all pending Views inside the <see cref="MainProcess"/> instance.
        /// </summary>
        public virtual void AfterLoading()
        {
            /*foreach (IView view in Parent.ActiveViews)
                if (!view.WasInitialized)
                    view.Init();*/
            AreResourceLoaded = true;
            Parent.LoadingScreen = null;
        }

        private async Task LoadContent()
        {
            await LoadResources();
            AreResourceLoaded = true;
        }

        private async Task LoadResources()
        {
            foreach (GlobalResourceLoader loader in ResourceLoaders)
            {
                TotalResourcesLoadedNum += await loader.LoadResources();
            }
            ResourcesLoaded.Invoke();
        }

        public void AddView<T>(params object[] viewParams)
        {
            GAME.AddView<T>(viewParams);
        }

        public List<IResource> GetGlobalResources() => Parent.ResourcePool;
    }
}
