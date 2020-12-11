namespace Joonaxii.ConsoleBulletHell
{
    public class WaitForSeconds : YieldInstruction
    {
        private float _time;
        private float _duration;

        public WaitForSeconds(Time space, float duration)
        {
            Setup(space);
            _duration = duration;
            _time = 0;
        }

        public override bool KeepWaiting
        {
            get
            {
                _time += _timeSpace.DeltaTime;
                return _time < _duration;
            }
        }
    }
}