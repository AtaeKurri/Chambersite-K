using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Interfaces
{
    public interface ICoroutineConsumer
    {
        public ICoroutineManager CoroutineManager { get; }

        public bool IsValid();
    }
}
