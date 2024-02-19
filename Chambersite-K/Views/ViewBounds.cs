using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public struct ViewBounds
    {
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Size { get; set; } = new Vector2(384, 448);
        public Vector2 BoundsOffsets { get; set; } = new Vector2(8, 16);

        public readonly int WorldLeft => (int)(Position.X - (Size.X / 2));
        public readonly int WorldRight => (int)(Position.X + (Size.X / 2));
        public readonly int WorldTop => (int)(Position.Y - (Size.Y / 2));
        public readonly int WorldBottom => (int)(Position.Y + (Size.Y / 2));

        public ViewBounds() { }

        public ViewBounds(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public ViewBounds(Vector2 position, Vector2 size, Vector2 boundsOffset)
            : this(position, size)
        {
            BoundsOffsets = boundsOffset;
        }

        public readonly RectangleF ToRectangleF() => new RectangleF(Position, Size);
        public static ViewBounds FromRectangleF(RectangleF rectangle) => new ViewBounds(rectangle.Position, rectangle.Size);
    }
}
