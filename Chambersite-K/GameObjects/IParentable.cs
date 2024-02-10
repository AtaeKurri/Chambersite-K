using Chambersite_K.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameObjects
{
    public interface IParentable
    {
        public object Parent { get; set; }
        public IView ParentView { get; set; }
        public List<GameObject> Children { get; set; }

        public void AddChild(GameObject child);
    }
}
