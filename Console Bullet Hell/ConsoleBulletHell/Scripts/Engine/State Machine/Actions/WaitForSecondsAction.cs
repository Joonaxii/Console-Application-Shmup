using System;
using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public class WaitForSecondsAction : StateAction
    {
        private float _waitFor;
        private FloatRange _range;
        
        public WaitForSecondsAction(float f) : base(true, true)
        {
            _waitFor = f;
        }

        public override StateAction Clone()
        {
            return new WaitForSecondsAction(_waitFor);
        }

        public override bool OnUpdate(float deltaTime)
        {
            time += deltaTime;

            if(time >= _waitFor)
            {
                return true;
            }

            return false;
        }
    }
}