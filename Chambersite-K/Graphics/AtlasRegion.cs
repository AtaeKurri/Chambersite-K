using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public class AtlasRegion : IEnumerable<Rectangle>
    {
        public Dictionary<int, Rectangle> Region { get; set; }
        public int RegionCount { get; set; } = 0;

        public Rectangle this[int key]
        {
            get
            {
                return Region[key];
            }
            set
            {
                Region[key] = value;
            }
        }

        public AtlasRegion()
        {
            Region = new Dictionary<int, Rectangle>();
        }

        public Rectangle AddEntry(int positionX, int positionY, int sizeX, int sizeY)
        {
            Rectangle rec = new Rectangle(positionX, positionY, sizeX, sizeY);
            Region.Add(RegionCount, rec);
            RegionCount++;
            return rec;
        }

        public Rectangle AddEntry(Vector2 position, Vector2 size)
        {
            return AddEntry((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public IEnumerator<Rectangle> GetEnumerator()
        {
            foreach (var region in Region)
            {
                yield return region.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
