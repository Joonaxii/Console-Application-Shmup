using System;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    /// <summary>
    /// This class holds the data for a given sprite
    /// </summary>
    public class Sprite
    {
        public Vector2 PivotOffset { get => new Vector2(_pivotOffsetX, _pivotOffsetY); }

        public int Width { get; }
        public int Height { get; }

        public int Order { get; private set; }

        #region Private Fields

        private float _pivotOffsetX;
        private float _pivotOffsetY;

        private char[] _chars;

        private bool _isMasker;
        private char[] _mask;

        private int _resXH;
        private int _resYH;

        #endregion

        /// <summary>
        /// Creates a sprite based on manually given data
        /// </summary>
        /// <param name="x">Sprite's Width</param>
        /// <param name="y">Sprite's Height</param>
        /// <param name="sprt">The chars the sprite is made out of, must be equal in length to X * Y</param>
        /// <param name="ord">Base rendering order</param>
        /// <param name="xOff">Pivot offset X (world coordinates)</param>
        /// <param name="yOff">Pivot offset Y (world coordinates)</param>
        public Sprite(int x, int y, char[] sprt, int ord, float xOff, float yOff)
        {
            Order = ord;

            _pivotOffsetX = xOff;
            _pivotOffsetY = yOff;

            Width = x;
            Height = y;

            _resXH = Width <= 1 ? 0 : Width % 2 == 0 ? Width / 2 : (Width - 1) / 2;
            _resYH = Height <= 1 ? 0 : Height % 2 == 0 ? Height / 2 : (Height - 1) / 2;

            _chars = sprt;
        }

        /// <summary>
        /// Creates a sprite based on given sprite data
        /// </summary>
        /// <param name="data">Data used to build the sprite, ususally this is the data that's loaded from disk or resources</param>
        public Sprite(SpriteData data)
        {
            Order = data.order;

            Width = data.resX;
            Height = data.resY;

            _pivotOffsetX = data.pivotOffsetX;
            _pivotOffsetY = data.pivotOffsetY;

            _chars = new char[Width * Height];
            for (int i = 0; i < _chars.Length; i++)
            {
                _chars[i] = data.pixels[i];
            }

            _resXH = Width <= 1 ? 0 : Width % 2 == 0 ? Width / 2 : (Width - 1) / 2;
            _resYH = Height <= 1 ? 0 : Height % 2 == 0 ? Height / 2 : (Height - 1) / 2;

            _isMasker = data.isMasker;
            _mask = data.mask;
        }

        /// <summary>
        /// Draws the sprite at a given position(in world space) with a given order offset
        /// </summary>
        /// <param name="position">Position to draw the sprite at (in world space)</param>
        /// <param name="depth">The current depth buffer</param>
        /// <param name="state">Current state of the world</param>
        /// <param name="orderOffset">An amount which will be added to the sprite's own rendering order value</param>
        public void Draw(Vector2 position, int[] depth, WorldState state, int orderOffset)
        {
            //Convert sprite's position in world space to graphic/console space
            Vector2Int screenPos = new Vector2Int((int)(position.x + _pivotOffsetX) + Renderer.WORLD_HALF_X, (int)(position.y + _pivotOffsetY) + Renderer.WORLD_HALF_Y);

            //Iterate over sprite's resolution
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Vector2Int posP = screenPos;
                    posP.x += x - _resXH;
                    posP.y += y - _resYH;

                    //If the current "pixel" is out of bounds, skip
                    if (!Renderer.IsInBounds(posP)) { continue; }

                    int index = posP.y * Renderer.WORLD_RES_X + posP.x;
                    int indexC = y * Width + x;

                    int orderOff = Order + orderOffset;
                    //If the depth at the current position in graphic space is greater or equal to this sprite's renderig order, skip 
                    if (depth[index] >= (_isMasker ? -orderOff : orderOff)) { continue; }

                    //This is just here to make the player's Collider visible in when the player is behind other sprites
                    if (_isMasker && state.State[index] == _mask[indexC]) { continue; }
                    char c = _chars[indexC];

                    //Also skip if the "pixel" is supposed to be empty/transparent.
                    if (c == Renderer.EMPTY_PIXEL_CHAR) { continue; }

                    //Write depth and char to the state
                    depth[index] = orderOff;
                    state.State[index] = c == Renderer.BLACK_PIXEL_CHAR ? ' ' : c;
                }
            }
        }

        /// <summary>
        /// Dumps the sprite into the Output Window
        /// </summary>
        public void DumpToOutput()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"XY: {Width}/{Height}\nOrder: {Order}\n");
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(_chars[y * Width + x]);
                }

                sb.Append('\n');
            }
            System.Diagnostics.Debug.Print(sb.ToString());
        }
    }
}