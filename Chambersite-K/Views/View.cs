using Chambersite_K.GameObjects;
using Chambersite_K.GameObjects.Coroutines;
using Chambersite_K.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public enum ViewType // Must be in order since MainProcess.Render will iterate through these in order.
    {
        Background, // Can only have a single Background at once.
        Stage, // Can only have a single Stage view active at once.
        Interface, // Can have multiple interfaces.
        Menu, // Can have multiple menus. Renders on top of interfaces since this is also meant for the pause menus too.
        LoadingScreen, // Can only have one per MainProcess instance.
    }

    public enum ViewStatus
    {
        AwaitingInit,
        Active,
        Paused,
        Hidden
    }

    public abstract class View : IView
    {
        public string InternalName { get; set; }

        /// <summary>
        /// Defines the type of a View.<br/>
        /// Is <see cref="ViewType.Menu"/> by default.
        /// </summary>
        public ViewType vType { get; private set; }
        public Guid? Id { get; set; } = null;
        public ViewStatus ViewStatus { get; set; } = ViewStatus.AwaitingInit;
        public bool WasInitialized { get; private set; } = false;
        public long Timer { get; set; } = 0;
        public int RenderOrder { get; set; } = -999_999_999;
        public ViewBounds WorldBounds { get; set; } = new ViewBounds();

        /// <summary>
        /// Stores all the resources loaded from type view scope. Access it directly to render standalone images.<br/>
        /// See <see cref="ResourceExtensions.FindResource{T}(List{IResource}, string)"/> to find resources inside this List more easily.
        /// </summary>
        public List<IResource> ResourcePool { get; set; } = new List<IResource>();
        public GameObjectPool LocalObjectPool { get; set; }

        /// <summary>
        /// The Parent of a View is always the instanced <see cref="MainProcess"/> object.
        /// </summary>
        public object Parent { get; set; } = GAME; // TODO: Allow for other view to be the parent (for nesting scenes.)
        public IView ParentView { get; set; }
        public List<GameObject> Children { get; set; } = new List<GameObject>();
        public ICoroutineManager CoroutineManager { get; set; }

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public View()
        {
            ParentView = this;
            LocalObjectPool = new GameObjectPool(this);
            CoroutineManager = new CoroutineManager(this);

            ViewTypeAttribute viewTypeAttr = (ViewTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(ViewTypeAttribute));
            vType = (viewTypeAttr != null) ? viewTypeAttr.ViewType : ViewType.Stage;

            InternalNameAttribute viewNameAttr = (InternalNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(InternalNameAttribute));
            InternalName = (viewNameAttr != null) ? viewNameAttr.InternalName : $"{GetType().Name}";

            RenderOrderAttribute renderOrderAttr = (RenderOrderAttribute)Attribute.GetCustomAttribute(GetType(), typeof(RenderOrderAttribute));
            RenderOrder = (renderOrderAttr != null) ? renderOrderAttr.RenderOrder : -999_999_999;
        }

        ~View()
        {
            GAME.ActiveViews.Remove(this);
        }

        public override string ToString()
        {
            return $"\"{InternalName}\" ({GetType().Name}:{Id})";
        }

        public virtual void LoadResources()
        {
        }

        public virtual void Init()
        {
            WasInitialized = true;
            ViewStatus = ViewStatus.Active;
            Logger.Debug("View {0} Initialized.", InternalName);
        }

        public virtual void Frame(GameTime gameTime)
        {
            if (ViewStatus == ViewStatus.Hidden || ViewStatus == ViewStatus.Paused)
                return;

            LocalObjectPool.Frame(gameTime);
            
            Timer++;
        }

        public virtual void Render()
        {
            if (ViewStatus == ViewStatus.Hidden)
                return;
            LocalObjectPool.Render();
        }

        public List<IResource> GetGlobalResources() => GAME.ResourcePool;

        /// <summary>
        /// Creates a <see cref="GameObject"/> and adds it as a Child of this <see cref="IView"/>.<br/>
        /// If <paramref name="globalObject"/> is set to <c>true</c>, this object will not render.
        /// </summary>
        /// <typeparam name="T">The <see cref="GameObject"/> type to instanciate.</typeparam>
        /// <param name="globalObject">Is this meant to be a global object?</param>
        /// <param name="image">Optional base image to render this object.</param>
        /// <returns>The instanciated <typeparamref name="T"/> <see cref="GameObject"/>.</returns>
        public GameObject CreateGameObject<T>(bool globalObject = false, string image = null)
        {
            GameObject go;
            if (globalObject)
                go = AddGameObjectToList<T>(GAME.GlobalObjectPool);
            else
                go = AddGameObjectToList<T>(LocalObjectPool);
            AddChild(go);
            return go;
        }

        private GameObject AddGameObjectToList<T>(GameObjectPool pool)
        {
            GameObject go = pool.CreateGameObject<T>(this, this);
            if (go.RenderOrder == -999_999_999)
                go.RenderOrder = pool.GetAllObjectCount() - 1;
            pool.ObjectPool.Sort((x, y) => x.RenderOrder.CompareTo(y.RenderOrder));
            return go;
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }

        public void MoveWorldBounds(RectangleF bounds)
        {
            WorldBounds = ViewBounds.FromRectangleF(bounds);
        }

        public bool IsValid() => (ViewStatus != ViewStatus.AwaitingInit || ViewStatus != ViewStatus.Paused);
    }
}
