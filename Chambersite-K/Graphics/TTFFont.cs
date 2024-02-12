using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;

namespace Chambersite_K.Graphics
{
    public class TTFFont
    {
        public string TTFPath { get; set; }

        public TTFFont(string ttfPath)
        {

        }

        public void Render(string text, int fontSize)
        {
            SpriteFont spriteFont = LoadToSpriteFont(text, fontSize);
        }

        private SpriteFont LoadToSpriteFont(string text, int fontSize)
        {
            return null;
        }
    }
}
