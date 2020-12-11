namespace Joonaxii.ConsoleBulletHell
{
    public class SpawnPatternAction : StateAction
    {
        private int _loops = -1;
        private PatternInstance[] _instances;
        private int _currentPattern;
        private int _currentLoop;

        public SpawnPatternAction(int loops, PatternInstance[] instances, bool _everyFrame, bool _mustWait) : base(_everyFrame, _mustWait)
        {
            _loops = loops;
            _instances = instances;

            ignore = loops < 0 & _everyFrame;
        }

        public override void Setup(State source)
        {
            base.Setup(source);

            for (int i = 0; i < _instances.Length; i++)
            {
                _instances[i].Load(_hostEntity);
            }
        }

        public override StateAction Clone()
        {
            PatternInstance[] instances = new PatternInstance[_instances.Length];
            for (int i = 0; i < _instances.Length; i++)
            {
                instances[i] = _instances[i].Clone();
            }
            return new SpawnPatternAction(_loops, instances, everyFrame, mustWait);
        }

        public override bool OnEnter()
        {
            base.OnEnter();
            return false;
        }

        public override void Reset()
        {
            base.Reset();
            _currentPattern = 0;
            _currentLoop = 0;

            for (int i = 0; i < _instances.Length; i++)
            {
                _instances[i].Reset();
                _instances[i].Clear();
            }
        }

        public override bool OnUpdate(float deltaTime)
        {
            PatternInstance pattern = _instances[_currentPattern];
            bool done = pattern.Update(deltaTime, _hostEntity.Position);
            if (done)
            {
                _currentPattern++;
                if (_currentPattern >= _instances.Length)
                {
                    _currentLoop++;
                }

                bool doLoop = everyFrame & (_loops < 0 | _currentLoop <= _loops);
                _currentPattern = doLoop ? _currentPattern % _instances.Length : _currentPattern;
                if(everyFrame & _currentPattern == 0)
                {
                    for (int i = 0; i < _instances.Length; i++)
                    {
                        _instances[i].Reset();
                    }
                }
            }
            return _currentPattern >= _instances.Length;
        }
    }
}