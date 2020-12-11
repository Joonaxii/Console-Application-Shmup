using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public abstract class Condition<T, TT> where TT : Entity
    {
        public abstract bool IsTrue { get; }

        private string _property;
        private string _propertyComp;

        private PropertyInfo _propInfo;
        private FieldInfo _fieldInfo;

        private PropertyInfo _propInfoComp;
        private FieldInfo _fieldInfoComp;

        protected T _compareAgainst;

        protected bool _compareToProperty;
        private StateMachine _stateMachine;

        public Condition(string propName, T compareAgainst)
        {
            _property = propName;
            _compareToProperty = false;
            _compareAgainst = compareAgainst;

            _propInfoComp = null;
            _fieldInfoComp = null;
            _propertyComp = "";
        }

        public Condition(T value, string compareAgainst)
        {
             _compareToProperty = false;
            _propertyComp = compareAgainst;
            _compareAgainst = value;

            _property = "";
            _propInfo = null;
            _propInfoComp = null;
        }

        public Condition(string propName, string compareAgainst)
        {
            _property = propName;
            _propertyComp = compareAgainst;
            _compareToProperty = true;
        }

        public virtual void UpdatePropertyInfo(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
             stateMachine.TryGetProperty(_property, out _propInfo, out _fieldInfo);

            if (_compareToProperty)
            {
                stateMachine.TryGetProperty(_propertyComp, out _propInfoComp, out _fieldInfoComp);
            }
        }

        protected T GetValue()
        {
            return _stateMachine.GetValue<T, TT>(_propInfo, _fieldInfo);
        }

        protected T GetCompareValue()
        {
            return _stateMachine.GetValue<T, TT>(_propInfoComp, _fieldInfoComp);
        }

        protected bool CompareToProperty()
        {
            return _property == "" & _propertyComp != "";
        }
    }
}