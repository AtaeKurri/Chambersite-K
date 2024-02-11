using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    /// <summary>
    /// Allows to set a defined render order to any <see cref="IView"/> and <see cref="GameObjects"/>.<br/>
    /// By default, the render order will be the order the Views/GameObjects are created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RenderOrderAttribute : Attribute
    {
        public readonly int RenderOrder;

        public RenderOrderAttribute(int renderOrder)
        {
            this.RenderOrder = renderOrder;
        }
    }
}
