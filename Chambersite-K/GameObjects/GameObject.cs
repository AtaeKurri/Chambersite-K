using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public Guid? Id { get; set; } = null;
        public GameObjectStatus Status { get; set; } = GameObjectStatus.Active;
        /// <summary>
        /// Checks if this <see cref="GameObject"/> instance is local to a <see cref="View"/> or a Global object.
        /// </summary>
        public bool IsLocalToView { get; set; } = false;
        public long Timer { get; set; } = 0;
        public int RenderOrder { get; set; } = -999_999_999;

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

        public GameObject()
        {
            InternalNameAttribute goNameAttr = (InternalNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(InternalNameAttribute));
            InternalName = (goNameAttr != null) ? goNameAttr.InternalName : $"{GetType().Name}";

            RenderOrderAttribute renderOrderAttr = (RenderOrderAttribute)Attribute.GetCustomAttribute(GetType(), typeof(RenderOrderAttribute));
            RenderOrder = (renderOrderAttr != null) ? renderOrderAttr.RenderOrder : -999_999_999;
        }

        ~GameObject()
        {
            if (IsLocalToView)
                try { ParentView.LocalObjectPool.ObjectPool.Remove(this); } catch (Exception) { }
            else
                try { GAME.GlobalObjectPool.ObjectPool.Remove(this); } catch (Exception) { }
        }

        public override string ToString()
        {
            return $"\"{InternalName}\" ({GetType().Name}:{Id})";
        }

        public virtual void Init()
        {
            try { ImageTexture = Resource.FindResource<Texture2D>(Image, ParentView); }
            catch (KeyNotFoundException) { ImageTexture = null; } // The ImageTexture wasn't set properly. This is normal for some objects so ignore.
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
                go = AddGameObjectToList<T>(ParentView.LocalObjectPool);
            AddChild(go);
            return go;
        }

        private GameObject AddGameObjectToList<T>(GameObjectPool pool)
        {
            GameObject go = pool.CreateGameObject<T>(this, ParentView);
            if (go.RenderOrder == -999_999_999)
                go.RenderOrder = pool.GetAllObjectCount() - 1;
            pool.ObjectPool.Sort((x, y) => x.RenderOrder.CompareTo(y.RenderOrder));
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
