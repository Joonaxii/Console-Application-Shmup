using System;
using System.Collections.Generic;
using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public class State
    {
        public string name;
        public StateAction[] Actions;

        private StateMachine _stateMachine;
        private int _currentActiveAction;

        public State(string stateName, StateAction[] actions)
        {
            name = stateName;
            Actions = actions;

        }

        public void Setup(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;

            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i].Setup(this);
                Actions[i].OnCreate();
            }
        }

        public StateMachine GetStateMachine()
        {
            return _stateMachine;
        }

        public void OnEnter()
        {
            Reset();
            _currentActiveAction = 0;
            if (Actions.Length > 0 && (Actions[_currentActiveAction].OnEnter() || !Actions[_currentActiveAction].mustWait))
            {
                FindAndStartNextActiveAction();
            }
        }

        public void Reset()
        {
            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i].Reset();
            }
        }

        public bool Update(float deltaTime)
        {
            bool stateCompleted = true;
            StateAction action;
            for (int i = 0; i < Actions.Length; i++)
            {
                action = Actions[i];
                if (!action.isComplete)
                {
                    bool actionDone;
                    if (!action.startRan)
                    {
                        actionDone = action.OnEnter();
                        if (actionDone)
                        {
                            action.OnFinish();
                        }
                    }
                    else
                    {
                        actionDone = action.isComplete || action.OnUpdate(deltaTime);
                    }

                    stateCompleted &= action.ignore ? true : action.isComplete;
                    if (i == _currentActiveAction)
                    {
                        if (actionDone)
                        {
                            if (!action.isComplete)
                            {
                                action.OnFinish();
                            }
                            _currentActiveAction++;
                            FindAndStartNextActiveAction();
                            continue;
                        }
                        return false;
                    }

                    if (actionDone)
                    {
                        action.OnFinish();
                    }
                }
                stateCompleted &= action.ignore ? true : action.isComplete;
            }
            return stateCompleted;
        }

        private void FindAndStartNextActiveAction()
        {
            while (_currentActiveAction < Actions.Length && (!Actions[_currentActiveAction].mustWait || Actions[_currentActiveAction].startRan || Actions[_currentActiveAction].isComplete))
            {
                _currentActiveAction++;
            }

            if (_currentActiveAction < Actions.Length)
            {
                if (Actions[_currentActiveAction].OnEnter())
                {
                    Actions[_currentActiveAction].OnFinish();
                    _currentActiveAction++;
                    FindAndStartNextActiveAction();
                }
            }
        }

        public void OnExit()
        {
            StopAllActions();
            _currentActiveAction = 0;
        }

        public void MoveToState(StateAction action, Tuple<State, int> state)
        {
            _currentActiveAction = 0;
            StopAllActions();
            _stateMachine.GotoState(state);
        }

        public State Clone()
        {
            StateAction[] actions = new StateAction[Actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = Actions[i].Clone();
            }

            State state = new State(name, actions);
            return state;
        }

        public Entity GetOwner()
        {
            return _stateMachine.Owner;
        }

        public virtual void Clear()
        {
            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i].Clear();
            }
        }

        private void StopAllActions()
        {
            for (int i = 0; i < Actions.Length; i++)
            {
                if (!Actions[i].isComplete)
                {
                    Actions[i].OnFinish();
                }
                Actions[i].Reset();
            }
        }
    }
}