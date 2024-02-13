using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameSettings
{
    public class InternalConfig
    {
        public virtual Enum CollisionEnum { get; set; }
        private List<Tuple<object, object>> CollisionsBehaviour { get; set; } = new List<Tuple<object, object>>();

        public InternalConfig()
        {

        }

        public void SetCollisionEnum<T>() where T : Enum
        {
            //CollisionEnum = T;
        }

        public void AddCollisionBehaviour()
        {

        }
    }
}
