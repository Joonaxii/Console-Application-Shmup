namespace Joonaxii.ConsoleBulletHell
{
    public abstract class StateAction
    {
        public float time;
        public bool everyFrame;
        public bool mustWait;
        public bool ignore;

        public bool startRan;
        public bool isComplete;

        protected State _hostState;
        protected Entity _hostEntity;

        public StateAction(bool _everyFrame, bool _mustWait)
        {
            everyFrame = _everyFrame;
            mustWait = _mustWait;
            ignore = false;
        }

        public virtual void Setup(State source)
        {
            _hostState = source;
        }

        public virtual void Reset()
        {
            time = 0;
            startRan = false;
            isComplete = false;
        }

        public virtual void OnCreate() { }
        public virtual bool OnEnter()
        {
            Reset();
            _hostEntity = _hostState.GetOwner();
            startRan = true;
            return !everyFrame;
        }
        public abstract bool OnUpdate(float deltaTime);
        public virtual void OnFinish()
        {
            isComplete = true;
        }

        public abstract StateAction Clone();

        public virtual void Clear() { }
    }
}