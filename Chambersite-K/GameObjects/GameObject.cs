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
        public Vector2 Direction { get; private set; } = Vector2.Zero;
        public float Acceleration { get; set; } = 0.0f;
        public Vector2 AccelerationDir { get; set; } = Vector2.Zero;

        public Resource? Image { get; set; } = null;

        /// <summary>
        /// Differs from <see cref="Rotation"/> as this property is used for actual movement.
        /// </summary>
        public float Angle { get; set; } = 0.0f; // In radians
        public float AngleDegrees
        {
            get { return Angle * (180.0f / MathF.PI); }
            set { Angle = value * (MathF.PI * 180.0f); }
        }

        /// <summary>
        /// Differs from <see cref="Angle"/> as this property is used for image manipulation. Will sync
        /// with <see cref="Angle"/> is <see cref="SyncRotation"/> is set to <c>true</c>.
        /// </summary>
        public float Rotation { get; set; } = 0.0f; // In radians
        public float RotationDegrees
        {
            get { return Rotation * (180.0f / MathF.PI); }
            set { Rotation = value * (MathF.PI * 180.0f); }
        }
        public bool SyncRotation { get; set; } = false;
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
                Direction = new Vector2(MathF.Cos(Angle), MathF.Sin(Angle));
                Position += Direction * Velocity;
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
