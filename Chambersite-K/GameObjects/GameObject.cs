using Chambersite_K.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameObjects
{
    public enum GameObjectStatus
    {
        Active,
        Paused,
        Invalid
    }

    // GameObjects should never try to load resources from themselves
    public abstract class GameObject : IGameObject, IParentable
    {
        public long Id { get; set; } = -1;
        public GameObjectStatus Status { get; set; } = GameObjectStatus.Active;
        public long Timer { get; set; } = 0;

        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Velocity { get; set; } = 0.0f;
        public Vector2 Direction { get; set; } = Vector2.Zero;
        public float Acceleration { get; set; } = 0.0f;
        public Vector2 AccelerationDir { get; set; } = Vector2.Zero;

        public Resource? Image { get; set; } = null;
        public float Rotation { get; set; } = 0.0f; // In radians
        public Vector2 Scale { get; set; } = Vector2.One;

        public object Parent { get; set; } = null;
        public List<GameObject> Children { get; set; } = new List<GameObject>();

        public delegate void DestroyEventHandler();
        public event DestroyEventHandler OnDestroy;

        public GameObject() { }

        public virtual void Init()
        {

        }

        public virtual void Frame()
        {
            if (Status == GameObjectStatus.Active)
            {
                Timer++;
            }
        }

        public virtual void Render()
        {
            Image?.Render(Position, Rotation, Scale);
        }

        public virtual void Delete()
        {
            if (Status == GameObjectStatus.Invalid)
                return;
            Status = GameObjectStatus.Invalid;
            OnDestroy();
        }

        public virtual void Kill()
        {
            if (Status == GameObjectStatus.Invalid)
                return;
            Status = GameObjectStatus.Invalid;
            OnDestroy();
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }
    }
}
