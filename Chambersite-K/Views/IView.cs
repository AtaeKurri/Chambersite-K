using Chambersite_K.GameObjects;
using Chambersite_K.GameObjects.Coroutines;
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
    public interface IView : IParentable, IResourceHolder, ICoroutineConsumer, IGameCycle
    {
        public string InternalName { get; set; }
        public Guid? Id { get; set; }
        public ViewType ViewType { get; }
        public ViewStatus ViewStatus { get; set; }
        public bool Hidden { get; set; }
        public GameObjectPool ObjectPool { get; set; }
        public bool WasInitialized { get; }
        public int? RenderOrder { get; set; }
        public ViewBounds WorldBounds { get; set; }
    }
}
