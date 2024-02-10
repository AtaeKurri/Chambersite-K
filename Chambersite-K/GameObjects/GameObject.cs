using Chambersite_K.Graphics;
using Chambersite_K.Views;
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
        public string InternalName { get; set; }
        public long Id { get; set; } = -1;
        public GameObjectStatus Status { get; set; } = GameObjectStatus.Active;
        public long Timer { get; set; } = 0;

        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Velocity { get; set; } = 0.0f;
        public Vector2 Direction { get; private set; } = Vector2.Zero;
        public float Acceleration { get; set; } = 0.0f;
        public Vector2 AccelerationDir { get; set; } = Vector2.Zero;

        /// <summary>
        /// The name of the base image of this <see cref="GameObject"/>. Will try to find
        /// the images in both the local and global resource pool, will display nothing if the image isn't found.
        /// </summary>
        public string? Image { get; set; } = null;
        public Resource? ImageTexture { get; private set; } = null;

        /// <summary>
        /// Differs from <see cref="Rotation"/> as this property is used for actual movement.
        /// </summary>
        public float Angle { get; set; } = 0.0f; // In radians
        public float AngleDegrees
        {
            get { return RadToDegNormalized(Angle); }
            set { Angle = DegToRadNormalized(value); }
        }

        /// <summary>
        /// Differs from <see cref="Angle"/> as this property is used for image manipulation. Will sync
        /// with <see cref="Angle"/> is <see cref="SyncRotation"/> is set to <c>true</c>.
        /// </summary>
        public float Rotation { get; set; } = 0.0f; // In radians
        public float RotationDegrees
        {
            get { return RadToDegNormalized(Rotation); }
            set { Rotation = DegToRadNormalized(value); }
        }
        /// <summary>
        /// Syncs <see cref="Rotation"/> with <see cref="Angle"/>'s value if set to true.
        /// </summary>
        public bool SyncRotation { get; set; } = true;
        public Vector2 Scale { get; set; } = Vector2.One;

        public object Parent { get; set; } = null;
        public IView ParentView { get; set; }
        public List<GameObject> Children { get; set; } = new List<GameObject>();

        public bool CheckCollision { get; set; } = true;

        public delegate void DestroyEventHandler();
        public event DestroyEventHandler OnDestroy;

        public GameObject() { }

        public virtual void Init()
        {
            try { ImageTexture = Resource.FindResource<Texture2D>(Image, ParentView); }
            catch (KeyNotFoundException) { } // The ImageTexture wasn't set properly. This is normal for some objects so ignore.
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
            ImageTexture?.Render(Position, Rotation, Scale);
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

        public GameObject CreateGameObject<T>(bool globalObject = false, string image = null)
        {
            GameObject go;
            if (globalObject)
                go = GAME.GlobalObjectPool.CreateGameObject<T>(this, ParentView);
            else
                go = ParentView.LocalObjectPool.CreateGameObject<T>(this, ParentView);
            AddChild(go);
            return go;
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }

        private float RadToDegNormalized(float rad)
        {
            return rad * (180.0f / MathF.PI);
        }

        private float DegToRadNormalized(float deg)
        {
            float orientation = deg % 360;
            if (orientation < 0) orientation += 360;
            orientation = orientation * (MathF.PI / 180.0f);
            return orientation;
        }
    }
}
