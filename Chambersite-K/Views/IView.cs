using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Microsoft.Xna.Framework.Graphics;
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
        public long Id { get; set; }
        public ViewType vType { get; }
        public ViewStatus ViewStatus { get; set; }
        public GameObjectPool LocalObjectPool { get; set; }
        public bool WasInitialized { get; }
        public long Timer { get; set; }
        public void Init();
        public void Frame();
        public void Render();
    }
}
