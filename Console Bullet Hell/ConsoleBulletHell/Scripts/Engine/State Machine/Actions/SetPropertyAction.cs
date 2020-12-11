using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public class SetPropertyAction<T, TT> : StateAction where TT : Entity
    {
        private T _value;

        private PropertyInfo _propertyInfo;
        private FieldInfo _fieldInfo;
        private string _property;
        private StateMachine _stateMachine;

        public SetPropertyAction(string propertyName, T valueTo, bool _everyFrame) : base(_everyFrame, false)
        {
            _property = propertyName;
            _value = valueTo;
        }

        public override void Setup(State source)
        {
            base.Setup(source);
            _stateMachine = source.GetStateMachine();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _hostState.GetStateMachine().TryGetProperty(_property, out _propertyInfo, out _fieldInfo);
        }

        public override StateAction Clone()
        {
            return new SetPropertyAction<T, TT>(_property, _value, everyFrame);
        }

        public override bool OnEnter()
        {
            bool exit = base.OnEnter();
            SetValue();
            return exit;
        }

        public override bool OnUpdate(float deltaTime)
        {
            SetValue();
            return true;
        }

        private void SetValue()
        {
            _stateMachine.SetValue<T, TT>(_propertyInfo, _fieldInfo, _value);
        }
    }
}