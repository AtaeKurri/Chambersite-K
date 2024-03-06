using Chambersite_K;
using Chambersite_K.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Chambersite_K.GameObjects.Coroutines
{
    public class CoroutineManager : ICoroutineManager
    {
        public List<Coroutine> ActiveCoroutines { get; }

        public ICoroutineConsumer Consumer { get; private set; }

        public CoroutineManager(ICoroutineConsumer consumer)
        {
            Consumer = consumer;
            ActiveCoroutines = new List<Coroutine>();
        }

        public void Frame(GameTime gameTime)
        {
            foreach (Coroutine coroutine in ActiveCoroutines)
            {
                ProcessCoroutine(coroutine);
            }
        }

        protected virtual void ProcessCoroutine(Coroutine coroutine)
        {
            if (!coroutine.Routine.MoveNext() || coroutine.IsFinished || !Consumer.IsValid())
            {
                ActiveCoroutines.Remove(coroutine);
                StopCoroutine(coroutine);
            }
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = new Coroutine(routine);
            return StartCoroutine(coroutine);
        }
        public Coroutine StartCoroutine(Coroutine coroutine)
        {
            ActiveCoroutines.Add(coroutine);
            return coroutine;
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            List<Coroutine> coroutines = ActiveCoroutines.Where(
                x => x.Routine.ToString() == coroutine.ToString()).ToList();

            foreach (Coroutine c in coroutines)
                StopCoroutine(c);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                coroutine.IsFinished = true;
            }
        }

        public void ClearCoroutines()
        {
            foreach (Coroutine coroutine in ActiveCoroutines)
                StopCoroutine(coroutine);
        }
    }
}
