using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public class TextureAtlasBoundsException : Exception
    {
        public TextureAtlasBoundsException() { }
        public TextureAtlasBoundsException(string message) : base(message) { }
        public TextureAtlasBoundsException(string message, Exception inner) : base(message, inner) { }
    }
}
