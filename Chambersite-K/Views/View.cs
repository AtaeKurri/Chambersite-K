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
        Stage, // Can only have a single Stage view active at once.
        LoadingScreen, // Can only have one per MainProcess instance.
        Interface, // Can have multiple interfaces.
    }

    public enum ViewStatus
    {
        Active,
        Paused,
        Hidden
    }

    public abstract class View : IView, IResourceHolder, IParentable
    {
        /// <summary>
        /// Defines the type of a View.<br/>
        /// Is <see cref="ViewType.Menu"/> by default.
        /// </summary>
        public ViewType vType { get; private set; }
        public ViewStatus ViewStatus { get; set; } = ViewStatus.Active;
        public bool WasInitialized { get; private set; } = false;

        /// <summary>
        /// Stores all the resources loaded from type view scope. Access it directly to render standalone images.<br/>
        /// See <see cref="ResourceExtensions.FindResource{T}(List{Resource}, string)"/> to find resources inside this List more easily.
        /// </summary>
        public List<Resource> LocalResources { get; set; } = new List<Resource>();
        public GameObjectPool LocalObjectPool { get; set; } = new GameObjectPool();

        public object Parent { get; set; } = null;
        public List<GameObject> Children { get; set; } = new List<GameObject>();

        public View()
        {
            ViewTypeAttribute viewTypeAttr = (ViewTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(ViewTypeAttribute));
            vType = (viewTypeAttr != null) ? viewTypeAttr.ViewType : ViewType.Menu;
        }

        public virtual void Init()
        {
            WasInitialized = true;
        }

        public virtual void Frame()
        {
            if (ViewStatus == ViewStatus.Hidden || ViewStatus == ViewStatus.Paused)
                return;
            foreach (GameObject gameObject in LocalObjectPool.ObjectPool)
            {
                gameObject.Frame();
            }
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
                go = GAME.GlobalObjectPool.CreateGameObject<T>(this);
            else
                go = LocalObjectPool.CreateGameObject<T>(this);
            return go;
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }
    }
}
