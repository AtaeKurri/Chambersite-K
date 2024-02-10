using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public enum ViewType
    {
        Menu, // Can have multiple menus.
        Background, // Can only have a single Background at once. Will always render on the back on everything.
        Stage, // Can only have a single Stage view active at once. Will render in the middle.
        LoadingScreen, // Can only have one per MainProcess instance.
        Interface, // Can have multiple interfaces. Will always render on top of everything. (TODO:Decide later for the rendering order)
    }

    public enum ViewStatus
    {
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
        public long Id { get; set; } = -1;
        public ViewStatus ViewStatus { get; set; } = ViewStatus.Active;
        public bool WasInitialized { get; private set; } = false;
        public long Timer { get; set; } = 0;

        /// <summary>
        /// Stores all the resources loaded from type view scope. Access it directly to render standalone images.<br/>
        /// See <see cref="ResourceExtensions.FindResource{T}(List{Resource}, string)"/> to find resources inside this List more easily.
        /// </summary>
        public List<Resource> LocalResources { get; set; } = new List<Resource>();
        public GameObjectPool LocalObjectPool { get; set; } = new GameObjectPool(true);

        /// <summary>
        /// The Parent of a View is always the instanced <see cref="MainProcess"/> object.
        /// </summary>
        public object Parent { get; set; } = GAME; // TODO: Allow for other view to be the parent (for nesting scenes.)
        public IView ParentView { get; set; }
        public List<GameObject> Children { get; set; } = new List<GameObject>();

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public View()
        {
            ParentView = this;
            ViewTypeAttribute viewTypeAttr = (ViewTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(ViewTypeAttribute));
            vType = (viewTypeAttr != null) ? viewTypeAttr.ViewType : ViewType.Menu;

            InternalNameAttribute viewNameAttr = (InternalNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(InternalNameAttribute));
            InternalName = (viewNameAttr != null) ? viewNameAttr.InternalName : "NullName";
        }

        ~View()
        {
            GAME.ActiveViews.Remove(this);
        }

        public override string ToString()
        {
            return $"\"{InternalName}\" ({GetType()})";
        }

        public virtual void Init()
        {
            WasInitialized = true;
            Logger.Debug("View {0} Initialized.", InternalName);
        }

        public virtual void Frame()
        {
            if (ViewStatus == ViewStatus.Hidden || ViewStatus == ViewStatus.Paused)
                return;
            foreach (GameObject gameObject in LocalObjectPool.ObjectPool)
            {
                gameObject.Frame();
            }
            Timer++;
        }
        public virtual void Render()
        {
            if (ViewStatus == ViewStatus.Hidden)
                return;
            foreach (GameObject gameObject in LocalObjectPool.ObjectPool)
            {
                gameObject.Render();
            }
        }

        public Resource LoadLocalResource<T>(string name, string filepath)
        {
            Resource res = Resource.Load<T>(name, filepath);
            if (res != null)
                LocalResources.Add(res);
            return res;
        }

        public List<Resource> GetGlobalResources() => GAME.GlobalResource;

        public GameObject CreateGameObject<T>(bool globalObject = false, string image = null)
        {
            GameObject go;
            if (globalObject)
                go = GAME.GlobalObjectPool.CreateGameObject<T>(this, this);
            else
                go = LocalObjectPool.CreateGameObject<T>(this, this);
            AddChild(go);
            return go;
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }
    }
}
