using Chambersite_K.GameObjects.Coroutines;
using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chambersite_K.GameObjects
{
    public enum GameObjectGroup
    {
        None, // Equivalent of "Ghost" in LuaSTG, will do nothing and interact will nothing.
        EnemyBullet,
        Enemy,
        PlayerBullet,
        Player,
        Item,
        Indestructible, // Cannot be deleted by other objects, only by explicitely call the del/kill function on them.
    }

    public enum GameObjectStatus
    {
        Active,
        Paused,
        AwaitingDeletion,
    }

    // GameObjects should never try to load resources from themselves
    public abstract class GameObject : IParentable, ICollisionActor, ICoroutineConsumer, IGameCycle
    {
        #region Properties
        public string InternalName { get; set; }
        public Guid? Id { get; set; } = null;
        public GameObjectStatus Status { get; set; } = GameObjectStatus.Active;
        public virtual GameObjectGroup Group { get; set; } = GameObjectGroup.None;
        public long Timer { get; set; } = 0;
        public int RenderOrder { get; set; } = -999_999_999;
        public IShapeF Bounds { get; set; }
        public bool Hidden { get; set; } = false;

        public virtual Vector2 Position { get; set; } = Vector2.Zero;
        public virtual float Velocity { get; set; } = 0.0f;
        public virtual Vector2 Direction { get; private set; } = Vector2.Zero;
        public virtual float Acceleration { get; set; } = 0.0f;
        public virtual Vector2 AccelerationDir { get; set; } = Vector2.Zero;

        /// <summary>
        /// The name of the base image of this <see cref="GameObject"/>. Will try to find
        /// the images in both the local and global resource pool, will display nothing if the image isn't found.
        /// </summary>
        public string? Image { get; set; } = null;
        public Resource<Texture2D>? ImageTexture { get; private set; } = null;

        /// <summary>
        /// Differs from <see cref="Rotation"/> as this property is used for actual movement.
        /// </summary>
        public float Angle { get; set; } = 0f; // In radians
        public float AngleDegrees
        {
            get { return RadToDegNormalized(Angle); }
            set { Angle = DegToRadNormalized(value); }
        }

        /// <summary>
        /// Differs from <see cref="Angle"/> as this property is used for image manipulation. Will sync
        /// with <see cref="Angle"/> is <see cref="SyncRotation"/> is set to <c>true</c>.
        /// </summary>
        public float Rotation { get; set; } = 0f; // In radians
        public float RotationDegrees
        {
            get { return RadToDegNormalized(Rotation); }
            set { Rotation = DegToRadNormalized(value); }
        }
        /// <summary>
        /// Syncs <see cref="Rotation"/> with <see cref="Angle"/>'s value if set to true.
        /// </summary>
        public virtual bool SyncRotation { get; set; } = true;
        public virtual Vector2 Scale { get; set; } = Vector2.One;

        public GameObjectPool ParentPool { get; set; }
        public object Parent { get; set; } = null;
        public IView ParentView { get; set; }
        public List<GameObject> Children { get; set; } = [];

        public virtual bool CheckCollision { get; set; } = true;
        public virtual bool CheckBound { get; set; } = true;

        public delegate void DestroyEventHandler();
        public event DestroyEventHandler OnDestroy;

        public ICoroutineManager CoroutineManager { get; set; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public GameObject()
        {
            CoroutineManager = new CoroutineManager(this);

            InternalNameAttribute goNameAttr = (InternalNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(InternalNameAttribute));
            InternalName = (goNameAttr != null) ? goNameAttr.InternalName : $"{GetType().Name}";

            RenderOrderAttribute renderOrderAttr = (RenderOrderAttribute)Attribute.GetCustomAttribute(GetType(), typeof(RenderOrderAttribute));
            RenderOrder = (renderOrderAttr != null) ? renderOrderAttr.RenderOrder : -999_999_999;
        }

        public GameObject(GameObject parent)
            : this()
        {
            Parent = parent;
        }

        ~GameObject()
        {
            try { ParentPool.ObjectPool.Remove(this); } catch (Exception) { }
        }

        public override string ToString()
        {
            return $"\"{InternalName}\" ({GetType().Name}:{Id})";
        }

        public virtual void Initialize()
        {
            try { ImageTexture = ParentView.FindResource<Texture2D>(Image); }
            catch (KeyNotFoundException) { ImageTexture = null; Logger.Debug($"{this} ImageTexture null. This is considered normal behaviour."); }
            // The ImageTexture wasn't set properly. This is normal for some objects (e.g: objects not meant to be rendered) so ignore.
        }

        public virtual void BeforeUpdate()
        {

        }

        public virtual void Update()
        {
            Direction = new Vector2(MathF.Cos(Angle), MathF.Sin(Angle));
            //Position += Direction * Velocity;
        }

        public virtual void AfterUpdate()
        {
            Timer++;
        }

        public virtual void Draw()
        {
            ImageTexture?.Draw(Position, Rotation, Scale);
        }

        public void DrawCollision()
        {
            Type type = Bounds.GetType();
            switch (type)
            {
                case Type when type == typeof(RectangleF):
                    GAME._spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3);
                    break;
                case Type when type == typeof(CircleF):
                    GAME._spriteBatch.DrawCircle((CircleF)Bounds, 8, Color.Red, 3);
                    break;
            }
        }

        public virtual void Delete()
        {
            if (Status == GameObjectStatus.AwaitingDeletion)
                return;
            Status = GameObjectStatus.AwaitingDeletion;
            OnDestroy.Invoke();
        }

        public virtual void Kill()
        {
            if (Status == GameObjectStatus.AwaitingDeletion)
                return;
            Status = GameObjectStatus.AwaitingDeletion;
            OnDestroy.Invoke();
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            Logger.Debug("Collided");
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
            orientation *= MathF.PI / 180.0f;
            return orientation;
        }

        public bool IsValid() => Status == GameObjectStatus.Active;
    }
}
