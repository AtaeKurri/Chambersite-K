using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewNameAttribute : Attribute
    {
        public readonly string ViewName;

        public ViewNameAttribute(string viewName)
        {
            this.ViewName = viewName;
        }
    }
}
