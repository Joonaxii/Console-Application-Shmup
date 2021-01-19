using System;

namespace Joonaxii.ConsoleBulletHell
{
    public class MenuButton
    {
        private const float _SELECT_TIMER = 0.125f;
        private const int _MAX_LINES = 5;

        private readonly static string[] _lines;

        public bool IsEnabled { get => _onPressed != null; }

        private string _menuName;
        private Action _onPressed;
        private float _time;

        private bool _selected;
        private int _currentCount = 0;

        static MenuButton()
        {
            _lines = new string[_MAX_LINES+1];
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i] = new string('-', i);
            }
        }

        public MenuButton(string name, Action action)
        {
            _onPressed = action;
            _menuName = name;
        }

        public void Draw(int y, bool selected, float deltaTime, bool init)
        {
            if (init)
            {
                _selected = selected;
                _currentCount = selected ? _MAX_LINES : 0;
                _time = 0;
            }
            else
            {
                if (selected != _selected)
                {
                    _time = 0;
                    _selected = selected;
                }
                else
                {
                    if (selected ? (_currentCount == _MAX_LINES) : (_currentCount == 0)) { return; }
                    float n = _time / _SELECT_TIMER;
                    _currentCount = (int)Maths.Lerp(0, _MAX_LINES, Maths.SmoothStep(0, 1, Maths.Clamp(selected ? n : 1.0f - n, 0, 1)));
                    _time += deltaTime;
                }
            }

            Console.ForegroundColor = !IsEnabled ? ConsoleColor.DarkGray : selected ? ConsoleColor.Yellow : ConsoleColor.Gray;
            ref string pad = ref _lines[_currentCount];

            Console.SetCursorPosition(0, y);
            Console.WriteLine(Extensions.GetCentered($"<{pad}{_menuName}{pad}>").PadLeft(30).PadRight(30));

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Press()
        {
            _onPressed?.Invoke();
        }
    }
}