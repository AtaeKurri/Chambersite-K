using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameObjects
{
    public sealed class GameObjectPool
    {
        public List<GameObject> ObjectPool { get; set; } = new List<GameObject>();
        private long nextID = 0; 

        public GameObjectPool()
        {
            ObjectPool = new List<GameObject>();
        }

        public GameObject CreateGameObject<T>(IParentable parent, params object[] objectParams)
        {
            GameObject go = (GameObject)Activator.CreateInstance(typeof(T), args:objectParams);
            go.Id = nextID;
            ObjectPool.Add(go);
            nextID++;
            go.Parent = parent;
            go.Init();
            return go;
        }
    }
}
