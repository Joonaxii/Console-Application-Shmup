using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Joonaxii.ConsoleBulletHell
{
    public static class Input
    {
        private const int KeyPressed = 0x8000;

        [DllImport("user32.dll"), SuppressUnmanagedCodeSecurity]
        private static extern short GetKeyState(int key);

        private static InputState[] _states;
        private static InputState[] _prevStates;
        private static KeyCode[] _keys;

        private readonly static Dictionary<KeyCode, int> _keyToIndex;

        static Input()
        {
            _keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
            _keyToIndex = new Dictionary<KeyCode, int>();

            _states = new InputState[_keys.Length];
            _prevStates = new InputState[_keys.Length];

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i] = new InputState();
                _prevStates[i] = new InputState();

                _keyToIndex.Add(_keys[i], i);
            }
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return GetKeyState(key, 0);
        }

        public static bool GetKey(KeyCode key)
        {
            return GetKeyState(key, 1);
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return GetKeyState(key, 2);
        }

        private static bool GetKeyState(KeyCode key, int state = 0)
        {
            if (!Program.INPUTS_SUPPRESSED) { return false; }
            switch (state)
            {
                default:
                    return _states[_keyToIndex[key]].down;
                case 1:
                    return _states[_keyToIndex[key]].held;
                case 2:
                    return _states[_keyToIndex[key]].up;
            }
        }

        public static void Update()
        {
            for (int i = 0; i < _states.Length; i++)
            {
                _prevStates[i].Copy(_states[i]);
            }

            for (int i = 0; i < _states.Length; i++)
            {
                InputState state = _states[i];
                InputState prevState = _prevStates[i];

                bool currentState = (GetKeyState((int)_keys[i]) & KeyPressed) != 0;

                state.down = !prevState.pressed & currentState;
                state.held = prevState.pressed & currentState;
                state.up = prevState.pressed & !currentState;

                state.pressed = state.down | state.held;
            }
        }
    }
}