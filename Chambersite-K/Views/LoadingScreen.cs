using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public delegate void ResourcesLoadedEventHandler();

    /// <summary>
    /// Base class for handling loading global resources at the game's launch.<br/>
    /// Use <see cref="AddResourceLoader{GlobalResourceLoader}"/> to add resource loaders.
    /// </summary>
    public abstract class LoadingScreen : IResourceHolder, IGameCycle
    {
        public MainProcess Parent { get; set; }
        public long Timer { get; set; }
        public List<GlobalResourceLoader> ResourceLoaders { get; set; } = new List<GlobalResourceLoader>();
        public bool AreResourceLoaded { get; set; } = false;
        public List<IResource> ResourcePool { get; set; } = new List<IResource>();
        public int TotalResourcesLoadedNum { get; set; } = 0;

        /// <summary>
        /// A list of <see cref="IView"/> to load after all resources were loaded.<br/>
        /// Usually the main menu and/or other views your game needs to work.
        /// </summary>
        public List<IView> ViewsToLoad { get; protected set; } = [];

        public event ResourcesLoadedEventHandler ResourcesLoaded;

        public LoadingScreen()
        {

        }

        public virtual void Initialize()
        {
            Task.Run(LoadContent);
        }

        public virtual void BeforeUpdate()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void AfterUpdate()
        {
            if (AreResourceLoaded)
            {
                Parent.LoadingScreen = null;
                foreach (IView view in ViewsToLoad)
                    AddView(view);
            }
        }

        public virtual void Draw()
        {
            
        }

        public void AddResourceLoader(GlobalResourceLoader resLoader)
        {
            ResourceLoaders.Add(resLoader);
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
        }

        public void AddView(IView view)
        {
            GAME.ActiveViews.AddView(view);
        }

        public List<IResource> GetGlobalResources() => Parent.ResourcePool;
    }
}
