using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public class CallMethodAction<TT> : StateAction where TT : Entity
    {
        private MethodInfo _methodInfo;
        private string _method;
        private StateMachine _stateMachine;

        public CallMethodAction(string method, bool _everyFrame) : base(_everyFrame, false)
        {
            _method = method;
        }

        public override void Setup(State source)
        {
            base.Setup(source);
            _stateMachine = source.GetStateMachine();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _hostState.GetStateMachine().TryGetMethod(_method, out _methodInfo);
        }

        public override StateAction Clone()
        {
            return new CallMethodAction<TT>(_method, everyFrame);
        }

        public override bool OnEnter()
        {
            bool exit = base.OnEnter();
            _stateMachine.CallMethod<TT>(_methodInfo);
            return exit;
        }

        public override bool OnUpdate(float deltaTime)
        {
            if (everyFrame)
            {
                _stateMachine.CallMethod<TT>(_methodInfo);
                return false;
            }

            return true;
        }
    }
}