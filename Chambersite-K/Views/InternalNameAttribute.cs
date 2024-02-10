using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InternalNameAttribute : Attribute
    {
        public readonly string InternalName;

        public InternalNameAttribute(string internalName)
        {
            this.InternalName = internalName;
        }
    }
}
