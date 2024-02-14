using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.World
{
    public abstract class Camera<T> where T : struct
    {
        public T Position { get; set; }
        public T Target { get; set; }
        public T Up { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        protected float aspectRatio;
        protected float fieldOfView;
        protected float nearPlaneDistance;
        protected float farPlaneDistance;
    }
}
