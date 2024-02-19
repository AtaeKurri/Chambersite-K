using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public class TextureAtlas
    {
        public Texture2D Texture { get; set; }
        public Dictionary<int, Rectangle> Regions { get; set; }
        public int RegionCount => Regions.Count;

        public TextureAtlas(Texture2D texture, Vector2 position, Vector2 size, Vector2 columnAndRow)
        {
            this.Texture = texture;
            CreateRegions(position, size, columnAndRow);
        }

        private void CreateRegions(Vector2 position, Vector2 size, Vector2 ColumnRow)
        {
            if (((position.X + size.X) * ColumnRow.X > Texture.Width) || ((position.Y + size.Y) * ColumnRow.Y > Texture.Height))
                throw new TextureAtlasBoundsException("Cannot create atlases with out of bounds positions, size or rows/columns");

            int count = 0;
            for (int x = 0; x < ColumnRow.X; x++)
            {
                for (int y = 0; y < ColumnRow.Y; y++)
                {
                    Regions.Add(count, new Rectangle((int)position.X+x, (int)position.Y+y, (int)size.X, (int)size.Y));
                    count++;
                }
            }
        }

        public void Render(int index)
        {

        }
    }
}
