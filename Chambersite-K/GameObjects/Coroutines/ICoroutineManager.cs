using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameObjects.Coroutines
{
    public interface ICoroutineManager
    {
        public List<Coroutine> ActiveCoroutines { get; }
        public void Frame(GameTime gameTime);
        public Coroutine StartCoroutine(IEnumerator coroutine);
        public Coroutine StartCoroutine(Coroutine coroutine);
        public void StopCoroutine(Coroutine coroutine);
        public void StopCoroutine(IEnumerator coroutine);
        public void ClearCoroutines();
    }
}
