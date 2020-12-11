namespace Joonaxii.ConsoleBulletHell
{
    public class WaitAFrameAction : StateAction
    {
        private bool _wait;

        public WaitAFrameAction() : base(false, true)
        {
        }

        public override StateAction Clone()
        {
            return new WaitAFrameAction();
        }

        public override void Reset()
        {
            base.Reset();
            _wait = true;
        }

        public override bool OnUpdate(float deltaTime)
        {
            if (_wait)
            {
                _wait = false;
                return !mustWait;
            }
            return true;
        }
    }
}