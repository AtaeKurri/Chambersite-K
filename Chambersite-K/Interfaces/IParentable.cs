using Chambersite_K.GameObjects;
using Chambersite_K.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Interfaces
{
    public interface IParentable
    {
        public object Parent { get; set; }
        public View ParentView { get; set; }
        public List<GameObject> Children { get; set; }

        public void AddChild(GameObject child);
    }
}
