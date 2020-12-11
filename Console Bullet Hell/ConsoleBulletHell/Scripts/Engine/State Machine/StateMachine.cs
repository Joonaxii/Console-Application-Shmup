using System;
using System.Collections.Generic;
using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public class StateMachine : IPoolable
    {
        public Entity Owner { get; private set; }
        public State CurrentState { get; private set; }

        public bool IsActive { get; private set; }

        private bool _wait;
        private Dictionary<string, Tuple<State, int>> _states;
        private State[] _statesSource;
        private ObjectPool _pool;

        private Dictionary<string, PropertyInfo> _properties;
        private Dictionary<string, FieldInfo> _fields;
        private Dictionary<string, MethodInfo> _methods;

        private int _currentState;
        private Type _type;

        public StateMachine(State[] states)
        {
            _statesSource = states;
            _states = new Dictionary<string, Tuple<State, int>>();
        }

        public void LoadStateMachine(Type type) 
        {
            _type = type;
            _properties = new Dictionary<string, PropertyInfo>();
            _fields = new Dictionary<string, FieldInfo>();
            _methods = new Dictionary<string, MethodInfo>();

            MethodInfo[] methods = type.GetMethods();
            PropertyInfo[] props = type.GetProperties();
            FieldInfo[] fields = type.GetFields();

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];

                if (!prop.CanRead & !prop.CanWrite) { continue; }
                _properties.Add(prop.Name, prop);
            }

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.IsPrivate) { continue; }
                _fields.Add(field.Name, field);
            }

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                ParameterInfo[] paramS = method.GetParameters();

                if (method.IsPrivate | (paramS != null & paramS.Length > 0)) { continue; }
                _methods.Add(method.Name, method);
            }

            for (int i = 0; i < _statesSource.Length; i++)
            {
                if (_states.ContainsKey(_statesSource[i].name)) { continue; }
                _states.Add(_statesSource[i].name, new Tuple<State, int>(_statesSource[i], i));
            }
        }

        public void Setup(Entity owner)
        {
            Owner = owner;
            for (int i = 0; i < _statesSource.Length; i++)
            {
                _statesSource[i].Setup(this);
            }
        }

        public void GotoState(string stateName)
        {
            _states.TryGetValue(stateName, out Tuple<State, int> state);
            GotoState(state);
        }

        public Tuple<State, int> GetState(string name)
        {
            return _states.TryGetValue(name, out Tuple<State, int> state) ? state : null;
        }

        public bool TryGetProperty(string name, out PropertyInfo propInfo, out FieldInfo fieldInfo)
        {
            bool found = _properties.TryGetValue(name, out propInfo);
            fieldInfo = null;
            found |= !found && _fields.TryGetValue(name, out fieldInfo);
            return found;
        }

        public bool TryGetMethod(string name, out MethodInfo methodInfo)
        {
            return _methods.TryGetValue(name, out methodInfo); ;
        }

        public void CallMethod<TT>(MethodInfo methodInfo) where TT : Entity
        {
            methodInfo?.Invoke(Owner as TT, null);
        }

        public void SetValue<T, TT>(PropertyInfo infoProp, FieldInfo infoField, T value) where TT : Entity
        {
            TT owner = Owner as TT;

            infoProp?.SetValue(owner, value);
            infoField?.SetValue(owner, value);
        }

        public T GetValue<T, TT>(PropertyInfo infoProp, FieldInfo infoField) where TT : Entity
        {
            return infoProp != null ? (T)infoProp.GetValue(Owner) : infoField != null ? (T)infoField.GetValue(Owner as TT) : default(T);
        }

        public void GotoState(Tuple<State, int> state)
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit();
                CurrentState = null;
            }

            if (state == null)
            {
                Stop();
                return;
            }

            CurrentState = state.Item1;
            _currentState = state.Item2;
            CurrentState.OnEnter();

            _wait = true;
        }

        public void Update(float deltaTime)
        {
            if (!IsActive | CurrentState == null) { return; }
            if (_wait) { _wait = false; return; }

            if (CurrentState.Update(deltaTime))
            {
                _currentState++;
                if(_currentState >= _statesSource.Length)
                {
                    Stop();
                    return;
                }

                CurrentState = _statesSource[_currentState];
                CurrentState.OnEnter();
            }
        }

        public void Start()
        {
            CurrentState = _statesSource[_currentState = 0];
            CurrentState?.OnEnter();

            _wait = true;
            IsActive = true;
        }

        public void Start(string stateName)
        {
            if(!_states.TryGetValue(stateName, out Tuple<State, int> state)) { Stop(); return; }
            Start(state);
        }

        private void Start(Tuple<State, int> state)
        {
            CurrentState = state.Item1;
            _currentState = state.Item2;
            CurrentState?.OnEnter();

            _wait = true;
            IsActive = true;
        }

        public void Stop()
        {
            CurrentState?.OnExit();
            IsActive = false;
            CurrentState = null;
        }

        public void UnLink()
        {
            _pool?.Return(this);

            for (int i = 0; i < _statesSource.Length; i++)
            {
                _statesSource[i].Clear();
            }
            Owner = null;
        }

        public StateMachine Clone()
        {
            State[] states = new State[_statesSource.Length];
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = _statesSource[i].Clone();
            }

            StateMachine machine = new StateMachine(states);

            machine._properties = _properties;
            machine._fields = _fields;
            machine._methods = _methods;

            machine.LoadStateMachine(_type);
            return machine;
        }

        public void Create(ObjectPool pool)
        {
            _pool = pool;
        }
    }
}