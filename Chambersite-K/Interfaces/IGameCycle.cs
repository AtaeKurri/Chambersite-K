using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Interfaces
{
    public interface IGameCycle
    {
        public long Timer { get; set; }
        public void Initialize();
        public void BeforeUpdate();
        public void Update();
        public void AfterUpdate();
        public void Draw();
    }
}
