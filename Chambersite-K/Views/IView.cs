using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public interface IView : IParentable, IResourceHolder
    {
        public string InternalName { get; set; }
        public Guid? Id { get; set; }
        public ViewType vType { get; }
        public ViewStatus ViewStatus { get; set; }
        public GameObjectPool LocalObjectPool { get; set; }
        public bool WasInitialized { get; }
        public long Timer { get; set; }
        public int RenderOrder { get; set; }
        public RectangleF WorldBounds { get; set; }
        public void Init();
        public void Frame(GameTime gameTime);
        public void Render();
    }
}
