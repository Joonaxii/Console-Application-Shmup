using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    public class WorldState
    {
        public char[] State = new char[Renderer.WORLD_RES_X * Renderer.WORLD_RES_Y];
        public bool[] StateChanged = new bool[Renderer.WORLD_RES_X * Renderer.WORLD_RES_Y];

        private StringBuilder _output = new StringBuilder(new string(' ', Renderer.WORLD_RES_X * Renderer.WORLD_RES_Y));

        public WorldState()
        {
            Clear();
        }

        public void Copy(WorldState state)
        {
            for (int i = 0; i < State.Length; i++)
            {
                State[i] = state.State[i];
            }

        }

        public void Compare(WorldState state)
        {
            for (int i = 0; i < State.Length; i++)
            {
                StateChanged[i] = State[i] != state.State[i];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < State.Length; i++)
            {
                State[i] = ' ';
            }
        }

        public void Draw(bool force)
        {
            //Iterate over the whole world
            for (int i = 0; i < State.Length; i++)
            {
                if (!StateChanged[i]) { continue; }
                _output[i] = State[i];
            }

            //Set the cursor position to the world's top left corner and write it
            Console.SetCursorPosition(Renderer.WORLD_CENTER_X, Renderer.WORLD_CENTER_Y);
            Console.Write(_output.ToString());
        }
    }
}