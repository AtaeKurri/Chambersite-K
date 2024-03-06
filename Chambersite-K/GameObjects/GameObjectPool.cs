using Chambersite_K.Interfaces;
using Chambersite_K.Views;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Collisions;
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
        private readonly CollisionComponent CollisionChecker;
        private HashSet<Guid> UsedGuids = new HashSet<Guid>();
        private object Parent;

        public GameObjectPool(MainProcess parent)
        {
            Parent = parent;
            CollisionChecker = new CollisionComponent(new MonoGame.Extended.RectangleF(
                MainProcess.Settings.ViewportSize.X,
                MainProcess.Settings.ViewportSize.Y,
                384,
                224
            ));
        }

        public GameObjectPool(View parent)
        {
            Parent = parent;
            CollisionChecker = new CollisionComponent(parent.WorldBounds.ToRectangleF());
        }

        public void BeforeUpdate()
        {
            for (int i = 0; i < ObjectPool.Count; i++)
            {
                GameObject go = ObjectPool[i];
                if (go.IsValid()) go.BeforeUpdate();
            }
        }

        public void Update()
        {
            for (int i = 0; i < ObjectPool.Count; i++)
            {
                GameObject go = ObjectPool[i];
                if (go.IsValid()) go.Update();
            }
        }

        public void AfterUpdate()
        {
            for (int i = 0; i < ObjectPool.Count; i++)
            {
                GameObject go = ObjectPool[i];
                if (go.IsValid()) go.AfterUpdate();
            }
            ObjectPool.RemoveAll(x => x.Status == GameObjectStatus.AwaitingDeletion);
        }

        public void Draw()
        {
            for (int i = 0; i<ObjectPool.Count; i++)
            {
                GameObject go = ObjectPool[i];
                if (go.IsValid() && !go.Hidden) go.Draw();
            }
        }

        public GameObject AddGameObject(GameObject gameObject, IParentable parent, View parentView)
        {
            GenerateGuid(ref gameObject);
            gameObject.ParentPool = this;
            gameObject.Parent ??= parent;
            gameObject.ParentView = parentView;
            ObjectPool.Add(gameObject);
            gameObject.Initialize();
            CollisionChecker.Insert(gameObject);

            if (gameObject.RenderOrder == null)
                gameObject.RenderOrder = GetAllObjectCount() - 1;
            ObjectPool.Sort((x, y) => Comparer<int>.Default.Compare((int)x.RenderOrder, (int)y.RenderOrder));
            return gameObject;
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
