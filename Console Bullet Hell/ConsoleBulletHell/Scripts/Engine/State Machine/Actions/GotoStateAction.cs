using System;

namespace Joonaxii.ConsoleBulletHell
{
    public class GotoStateAction<T, TT> : StateAction where TT : Entity
    {
        private string _targetState;
        private Tuple<State, int> _state;
        private Condition<T, TT>[] _conditions;
        private WeightedObject<string>[] _weightedStateNames;
        private WeightedCollection<Tuple<State, int>> _weightedStates;
        private bool _requireAll;

        public GotoStateAction(string targetState) : this(targetState, false, false, null)
        {
        }

        public GotoStateAction(WeightedObject<string>[] targetState) : this(targetState, false, false, null)
        {
           
        }

        public GotoStateAction(string targetState, bool requireAll, bool everyFrame, params Condition<T, TT>[] conditions) : base(everyFrame, false)
        {
            _targetState = targetState;
            _conditions = conditions;
            _requireAll = requireAll;
            _weightedStateNames = null;
            _weightedStates = null;
        }

        public GotoStateAction(WeightedObject<string>[]  targetState, bool requireAll, bool everyFrame, params Condition<T, TT>[] conditions) : base(everyFrame, false)
        {
            _conditions = conditions;
            _requireAll = requireAll;
            _weightedStateNames = targetState;
        }

        public override void Setup(State source)
        {
            base.Setup(source);

            if(_conditions == null) { return; }
            StateMachine _machine = source.GetStateMachine();
            for (int i = 0; i < _conditions.Length; i++)
            {
                _conditions[i].UpdatePropertyInfo(_machine);
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();

            StateMachine _machine = _hostState.GetStateMachine();

            if (_weightedStateNames != null)
            {
                WeightedObject<Tuple<State, int>>[] _states = new WeightedObject<Tuple<State, int>>[_weightedStateNames.Length];
                for (int i = 0; i < _states.Length; i++)
                {
                    _states[i] = new WeightedObject<Tuple<State, int>>(_machine.GetState(_weightedStateNames[i].item), _weightedStateNames[i].weight);
                }

                _weightedStates = new WeightedCollection<Tuple<State, int>>(true, _states);
                return;
            }

            _state = _machine.GetState(_targetState);
        }

        public override StateAction Clone()
        {
            return _weightedStateNames != null ? new GotoStateAction<T, TT>(_weightedStateNames, _requireAll, everyFrame, _conditions) : new GotoStateAction<T, TT>(_targetState, _requireAll, everyFrame, _conditions);
        }

        public override bool OnEnter()
        {
            bool exitEarly = base.OnEnter();
            if (_conditions != null)
            {
                bool conditionsMet = _requireAll;
                for (int i = 0; i < _conditions.Length; i++)
                {
                    bool isMet = _conditions[i].IsTrue;
                    if (_requireAll & !isMet)
                    {
                        conditionsMet = false;
                        break;
                    }

                    if(!_requireAll & isMet)
                    {
                        conditionsMet = true;
                        break;
                    }
                }

                if (conditionsMet)
                {
                    GotoState();
                }
                return exitEarly & conditionsMet;
            }

            GotoState();
            return exitEarly;
        }

        public override bool OnUpdate(float deltaTime)
        {
            if (_conditions != null)
            {
                bool conditionsMet = _requireAll;
                for (int i = 0; i < _conditions.Length; i++)
                {
                    bool isMet = _conditions[i].IsTrue;
                    if (_requireAll & !isMet)
                    {
                        conditionsMet = false;
                        break;
                    }

                    if (!_requireAll & isMet)
                    {
                        conditionsMet = true;
                        break;
                    }
                }

                if (conditionsMet)
                {
                    GotoState();
                }
                return false;
            }
            return true;
        }

        private void GotoState()
        {
            if(_weightedStates != null)
            {
                _hostState.MoveToState(this, _weightedStates.SelectRandom());
                return;
            }
            _hostState.MoveToState(this, _state);
        }
    }
}