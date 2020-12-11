namespace Joonaxii.ConsoleBulletHell
{
    public class InputState
    {
        public bool down;
        public bool held;
        public bool up;
        public bool pressed;

        public void Reset()
        {
            down = false;
            held = false;
            up = false;
            pressed = false;
        }

        public void Copy(InputState state)
        {
            down = state.down;
            held = state.held;
            up = state.up;
            pressed = state.pressed;
        }
    }
}