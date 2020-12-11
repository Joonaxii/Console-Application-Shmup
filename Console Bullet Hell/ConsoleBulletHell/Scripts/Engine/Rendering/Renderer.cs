using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Joonaxii.ConsoleBulletHell
{
    public static class Renderer
    {
        public static int CurrentWidth { get; private set; }
        public static int CurrentHeight { get; private set; }

        public const char EMPTY_PIXEL_CHAR = '½';
        public const char BLACK_PIXEL_CHAR = '^';

        public const int SCREEN_RES_X = (int)(RES_X * 0.65f);
        public const int SCREEN_RES_Y = (int)(RES_Y * 0.85f);

        public const int WORLD_SIZE = WORLD_RES_X * WORLD_RES_Y;

        public const int WORLD_RES_X = SCREEN_RES_X;
        public const int WORLD_RES_Y = SCREEN_RES_Y - (int)(15 * 0.85f);

        public static List<Entity> RenderableEntities = new List<Entity>();

        public static bool LOADED = false;

        private const int RES_X = 240;
        private const int RES_Y = 62;

        private const int X_OFFSET = 0;
        private const int Y_OFFSET = 5;

        public const int WORLD_HALF_X = WORLD_RES_X / 2;
        public const int WORLD_HALF_Y = WORLD_RES_Y / 2;

        public const int WORLD_CENTER_X = ((SCREEN_RES_X - WORLD_RES_X) / 2) + X_OFFSET;
        public const int WORLD_CENTER_Y = ((SCREEN_RES_Y - WORLD_RES_Y) / 2) + Y_OFFSET;

        private const int WORLD_MIN_X = WORLD_CENTER_X;
        private const int WORLD_MAX_X = WORLD_RES_X + WORLD_CENTER_X;

        private const int WORLD_MIN_Y = WORLD_CENTER_Y;
        private const int WORLD_MAX_Y = WORLD_RES_Y + WORLD_CENTER_Y;

        private static WorldState _bufferState;
        private static WorldState _worldState;

        private const int CMD = 0x00000000;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        private static TextLine[] _lines;

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        private static int[] _depthBuffer = new int[WORLD_RES_X * WORLD_RES_Y];
        private static StoppableThread _thread;

        public static void Initialize(StoppableThread thread)
        {
            LOADED = false;

            SetSize(SCREEN_RES_X, SCREEN_RES_Y);

            _lines = new TextLine[SCREEN_RES_Y];
            for (int i = 0; i < SCREEN_RES_Y; i++)
            {
                _lines[i] = new TextLine(i >= WORLD_RES_Y + WORLD_CENTER_Y + 1 | i <= WORLD_CENTER_Y - 1);
            }

            _thread = thread;
        }

        public static void SetSize(int width, int height)
        {
            CurrentWidth = Math.Min(width, Console.LargestWindowWidth);
            CurrentHeight = Math.Min(height, Console.LargestWindowHeight);

            Console.SetWindowSize(CurrentWidth, CurrentHeight);
            IntPtr consolePtr = GetConsoleWindow();

            DeleteMenu(GetSystemMenu(consolePtr, false), SC_MAXIMIZE, CMD);
            DeleteMenu(GetSystemMenu(consolePtr, false), SC_SIZE, CMD);
        }

        /// <summary>
        /// Checks if the given point lies within the world's draw area
        /// </summary>
        /// <param name="point">The coordinate to check in Console Space</param>
        /// <returns></returns>
        public static bool IsInBounds(Vector2Int point)
        {
            return point.x >= 0 & point.x < WORLD_RES_X & point.y >= 0 & point.y < WORLD_RES_Y;
        }

        public static void WriteText(string text, int y, ConsoleColor color = ConsoleColor.White)
        {
            if (!LOADED) { return; }
            if (y < 0 | y >= _lines.Length | !_lines[y].IsOK) { return; }
            _lines[y].SetText(text);
        }

        public static void ClearLine(int y)
        {
            if (!LOADED) { return; }
            if (y < 0 | y >= _lines.Length | !_lines[y].IsOK) { return; }
            _lines[y].MarkedForClear = true;
        }

        /// <summary>
        /// Adds an entity to the renderable entities List, does not check if it already is in the list
        /// you have to make sure it isn't before hand, or you have to know this method is only getting called once per renderabel
        /// </summary>
        /// <param name="id"></param>
        public static void AddRenderer(Entity id)
        {
            lock (RenderableEntities)
            {
                RenderableEntities.Add(id);
            }
        }

        /// <summary>
        /// The main draw method for the Renderer
        /// </summary>
        public static void Draw()
        {
            LOADED = false;
            _bufferState = new WorldState();
            _worldState = new WorldState();

            _bufferState.Copy(_worldState);

            DrawEdge(ConsoleColor.White);
            _worldState.Draw(true);

            TextLine line;

            LOADED = true;
            while (_thread.IsRunning)
            {
                for (int i = 0; i < _lines.Length; i++)
                {
                    line = _lines[i];
                    if (line.MarkedForClear)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write("".PadRight(SCREEN_RES_X-1));
                        line.Clear();
                        continue;
                    }

                    if (line.isDirty)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write(line.text.PadRight(SCREEN_RES_X-1));
                        line.written = true;
                        line.isDirty = false;
                    }
                }

                DrawWorld();
                Program.RenderTime.Tick();
            }
            _thread.ActuallyRunning = false;
        }

        private static void DrawWorld()
        {
            //Clear the Depth Buffer
            for (int i = 0; i < _depthBuffer.Length; i++)
            {
                _depthBuffer[i] = int.MinValue;
            }

            //Copy the current state and then clear it
            _bufferState.Copy(_worldState);
            _worldState.Clear();
        
            //Lock Sprites
            lock (RenderableEntities)
            {
                RenderableEntities.Sort(new SpriteOrderComparer());

                //Iterate over all sprites
                Entity entity;
                for (int i = 0; i < RenderableEntities.Count; i++)
                {
                    entity = RenderableEntities[i];
                    //If entity's sprite is null or if the entity isn't rendered, skip...
                    if (entity.Sprite == null | !entity.Render) { continue; }

                    //...otherwise draw/write the sprite to the current world state
                    entity.Sprite.Draw(entity.Position, _depthBuffer, _worldState, entity.renderingOffset);

                    //These are for debugging colliders
                    if(Program.DEBUG & entity.Type != EntityType.NONE & entity.CanCollide)
                    {
                        ColliderBase coll;
                        for (int j = 0; j < entity.Collider.CollisionShapes.Length; j++)
                        {
                            coll = entity.Collider.CollisionShapes[j];

                            if (coll.ShapeSprite != null)
                            {
                                coll.ShapeSprite.Draw(coll.Center, _depthBuffer, _worldState, entity.renderingOffset);   
                            }
                        }
                    }
                }
            }

            //Compare previous and current state
            _worldState.Compare(_bufferState);

            //Draw the state to the world
            _worldState.Draw(false);

            //These lines debug the MAX_X position that the player is allowed to move to
            //THIS IS SLOW!!! ONLY USE IT FOR DEBUGGING
            if (Program.DEBUG)
            {
                int xx = (int)(Player.MAX_X + WORLD_HALF_X);
                if(xx >= 0 & xx < WORLD_RES_X)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int y = 0; y < WORLD_RES_Y; y++)
                    {
                        int yy = y + WORLD_CENTER_Y;

                        Console.SetCursorPosition(xx, yy);
                        Console.Write('║');
                    }

                    Console.ResetColor();
                }
            } 
        }

        /// <summary>
        /// Draws the edge of the playable area in a specific Console Color
        /// </summary>
        /// <param name="color">The color used for drawing the edge with</param>        
        private static void DrawEdge(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.ForegroundColor = color;
            for (int x = 0; x < WORLD_RES_X; x++)
            {
                Console.SetCursorPosition(x + WORLD_CENTER_X, WORLD_CENTER_Y - 1);
                Console.Write("#");

                Console.SetCursorPosition(x + WORLD_CENTER_X, WORLD_RES_Y + WORLD_CENTER_Y);
                Console.Write("#");
            }
            Console.ResetColor();
        }
    }
}