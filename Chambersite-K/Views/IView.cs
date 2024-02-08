using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public interface IView
    {
        public ViewType vType { get; }
        public ViewStatus ViewStatus { get; set; }
        public bool WasInitialized { get; }
        public void Init();
        public void Frame();
        public void Render();
    }
}
