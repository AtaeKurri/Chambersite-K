using Chambersite_K.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameObjects
{
    public sealed class GameObjectPool : IEnumerable<GameObject>
    {
        public List<GameObject> ObjectPool { get; set; } = new List<GameObject>();
        private bool IsLocalPool = false;
        private HashSet<Guid> UsedGuids = new HashSet<Guid>();

        public GameObjectPool(bool isLocal)
        {
            IsLocalPool = isLocal;
        }

        public GameObject CreateGameObject<T>(IParentable parent, IView parentView, params object[] objectParams)
        {
            GameObject go = (GameObject)Activator.CreateInstance(typeof(T), args:objectParams);
            GenerateGuid(ref go);
            go.IsLocalToView = IsLocalPool;
            go.Parent = parent;
            go.ParentView = parentView;
            ObjectPool.Add(go);
            go.Init();
            return go;
        }

        private void GenerateGuid(ref GameObject go)
        {
            Guid uuid = Guid.NewGuid();

            while (UsedGuids.Contains(uuid))
                uuid = Guid.NewGuid();

            go.Id = uuid;
            UsedGuids.Add(uuid);
        }

        public int GetAllObjectCount() => ObjectPool.Count;

        public IEnumerator<GameObject> GetEnumerator()
        {
            return ObjectPool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
