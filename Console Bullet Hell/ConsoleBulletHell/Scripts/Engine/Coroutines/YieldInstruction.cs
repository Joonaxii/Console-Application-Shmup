using System.Collections;

namespace Joonaxii.ConsoleBulletHell
{
    public abstract class YieldInstruction : IEnumerator
    {
        protected Time _timeSpace;
        public abstract bool KeepWaiting { get; }

        public object Current => null;
        public bool MoveNext()
        {
            return KeepWaiting;
        }

        public void Setup(Time timeSpace) { _timeSpace = timeSpace; }
        public void Reset() { }
 
    }
}