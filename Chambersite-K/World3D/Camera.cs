using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Chambersite_K.World3D
{
    public abstract class Camera<T>
        where T : struct
    {
        public abstract T Position { get; set; }
        public abstract float Rotation { get; set; }
        public abstract float FOV { get; set; }
        public abstract RectangleF BoundingRectangle { get; set; }
        public abstract T Origin { get; set; }
        public abstract T Center { get; set; }

        public abstract void Move(T direction);
        public abstract void Rotate(float deltaRadians);
        public abstract void SetFOV(float fov);
        public abstract void LookAt(T position);

        public abstract T WorldToScreen(T worldPosition);
        public abstract T ScreenToWorld(T screenPosition);

        public abstract Matrix GetViewMatrix();
        public abstract Matrix GetInverseViewMatrix();

        public abstract BoundingFrustum GetBoundingFrustum();
        public abstract ContainmentType Contains(Vector2 vector);
        public abstract ContainmentType Contains(Rectangle rectangle);
    }
}
