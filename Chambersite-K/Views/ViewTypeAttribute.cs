using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewTypeAttribute : Attribute
    {
        public readonly ViewType ViewType;

        public ViewTypeAttribute(ViewType viewType)
        {
            this.ViewType = viewType;
        }
    }
}
