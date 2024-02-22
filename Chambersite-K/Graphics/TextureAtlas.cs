using Chambersite_K.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public class TextureAtlas(Texture2D texture) : IEnumerable<AtlasRegion>
    {
        public Texture2D Texture { get; set; } = texture;
        public Dictionary<string, AtlasRegion> Regions { get; set; }
        public int RegionCount => Regions.Count;

        public AtlasRegion this[string key]
        {
            get
            {
                return Regions[key];
            }
            set
            {
                Regions[key] = value;
            }
        }

        public AtlasRegion CreateRegion(string regionName, Vector2 position, Vector2 size, Vector2 ColumnRow)
        {
            if (((position.X + size.X) * ColumnRow.X > Texture.Width) || ((position.Y + size.Y) * ColumnRow.Y > Texture.Height))
                throw new TextureAtlasBoundsException("Cannot create atlases with out of bounds positions, size or rows/columns");

            AtlasRegion region = new();
            for (int x = 0; x < ColumnRow.X; x++)
            {
                for (int y = 0; y < ColumnRow.Y; y++)
                {
                    region.AddEntry((int)position.X+x, (int)position.Y+y, (int)size.X, (int)size.Y);
                }
            }
            Regions.Add(regionName, region);
            return region;
        }

        public void Render(int index)
        {

        }

        public IEnumerator<AtlasRegion> GetEnumerator()
        {
            foreach (var region in Regions)
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
