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
    }

    public enum ViewStatus
    {
        AwaitingInit,
        Active,
        Paused
    }

    public abstract class View : IView
    {
        public string InternalName { get; set; }

        /// <summary>
        /// Defines the type of a View.<br/>
        /// Is <see cref="ViewType.Menu"/> by default.
        /// </summary>
        public ViewType ViewType { get; private set; }
        public Guid? Id { get; set; } = null;
        public ViewStatus ViewStatus { get; set; } = ViewStatus.AwaitingInit;
        public bool WasInitialized { get; private set; } = false;
        public long Timer { get; set; } = 0;
        public bool Hidden { get; set; } = false;
        public int RenderOrder { get; set; } = -999_999_999;
        public ViewBounds WorldBounds { get; set; } = new ViewBounds();

        /// <summary>
        /// Stores all the resources loaded from type view scope. Access it directly to render standalone images.<br/>
        /// See <see cref="ResourceExtensions.FindResource{T}(List{IResource}, string)"/> to find resources inside this List more easily.
        /// </summary>
        public List<IResource> ResourcePool { get; set; } = new List<IResource>();
        public GameObjectPool ObjectPool { get; set; }

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
            ObjectPool = new GameObjectPool(this);
            CoroutineManager = new CoroutineManager(this);

            ViewTypeAttribute viewTypeAttr = (ViewTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(ViewTypeAttribute));
            ViewType = (viewTypeAttr != null) ? viewTypeAttr.ViewType : ViewType.Stage;

            InternalNameAttribute viewNameAttr = (InternalNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(InternalNameAttribute));
            InternalName = (viewNameAttr != null) ? viewNameAttr.InternalName : $"{GetType().Name}";

            RenderOrderAttribute renderOrderAttr = (RenderOrderAttribute)Attribute.GetCustomAttribute(GetType(), typeof(RenderOrderAttribute));
            RenderOrder = (renderOrderAttr != null) ? renderOrderAttr.RenderOrder : -999_999_999;
        }

        ~View()
        {
            GAME.ActiveViews.Pool.Remove(this);
        }

        public override string ToString()
        {
            return $"\"{InternalName}\" ({GetType().Name}:{Id})";
        }

        public virtual void LoadResources()
        {
        }

        public virtual void Initialize()
        {
            WasInitialized = true;
            ViewStatus = ViewStatus.Active;
            Logger.Debug("View {0} Initialized.", InternalName);
        }

        public virtual void BeforeUpdate()
        {
            ObjectPool.BeforeUpdate();
        }

        public virtual void Update()
        {
            ObjectPool.Update();
        }

        public virtual void AfterUpdate()
        {
            ObjectPool.AfterUpdate();
            Timer++;
        }

        public virtual void Draw()
        {
            ObjectPool.Draw();
        }

        public List<IResource> GetGlobalResources() => GAME.ResourcePool;

        /// <summary>
        /// Creates a <see cref="GameObject"/> and adds it as a Child of this <see cref="IView"/>.<br/>
        /// If <paramref name="globalObject"/> is set to <c>true</c>, this object will not render and will be added to the global pool.
        /// </summary>
        /// <param name="globalObject">Is this meant to be a global object?</param>
        /// <returns>The <paramref name="gameObject"/> instance.</returns>
        public GameObject CreateGameObject(GameObject gameObject, bool globalObject = false)
        {
            AddGameObjectToList(gameObject, globalObject ? GAME.ObjectPool : ObjectPool);
            AddChild(gameObject);
            return gameObject;
        }

        private GameObject AddGameObjectToList(GameObject gameObject, GameObjectPool pool) => pool.AddGameObject(gameObject, this, this);

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }

        public void MoveWorldBounds(RectangleF bounds)
        {
            WorldBounds = ViewBounds.FromRectangleF(bounds);
        }

        public bool IsValid() => (ViewStatus != ViewStatus.AwaitingInit);
    }
}
