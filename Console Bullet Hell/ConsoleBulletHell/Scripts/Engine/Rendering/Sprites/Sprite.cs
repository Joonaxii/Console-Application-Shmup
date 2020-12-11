using System;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    /// <summary>
    /// This class holds the data for a given sprite
    /// </summary>
    public class Sprite
    {
        public int resX;
        public int resY;

        public float pivotOffsetX;
        public float pivotOffsetY;

        public char[] Chars;
    
        public int order;

        public bool isMasker;
        public char[] Mask;

        private int _resXH;
        private int _resYH;

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
            order = ord;

            pivotOffsetX = xOff;
            pivotOffsetY = yOff;

            resX = x;
            resY = y;

            _resXH = resX <= 1 ? 0 : resX % 2 == 0 ? resX / 2 : (resX - 1) / 2;
            _resYH = resY <= 1 ? 0 : resY % 2 == 0 ? resY / 2 : (resY - 1) / 2;

            Chars = sprt;
        }

        /// <summary>
        /// Creates a sprite based on given sprite data
        /// </summary>
        /// <param name="data">Data used to build the sprite, ususally this is the data that's loaded from disk or resources</param>
        public Sprite(SpriteData data)
        {
            order = data.order;

            resX = data.resX;
            resY = data.resY;

            pivotOffsetX = data.pivotOffsetX;
            pivotOffsetY = data.pivotOffsetY;

            Chars = new char[resX * resY];
            for (int i = 0; i < Chars.Length; i++)
            {
                Chars[i] = data.pixels[i];
            }

            _resXH = resX <= 1 ? 0 : resX % 2 == 0 ? resX / 2 : (resX - 1) / 2;
            _resYH = resY <= 1 ? 0 : resY % 2 == 0 ? resY / 2 : (resY - 1) / 2;

            isMasker = data.isMasker;
            Mask = data.mask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        /// <param name="state"></param>
        /// <param name="orderOffset"></param>
        public void Draw(Vector2 position, int[] depth, WorldState state, int orderOffset)
        {
            //Convert sprite's position in world space to graphic/console space
            Vector2Int screenPos = new Vector2Int((int)(position.x + pivotOffsetX) + Renderer.WORLD_HALF_X, (int)(position.y + pivotOffsetY) + Renderer.WORLD_HALF_Y);

            //Iterate over sprite's resolution
            for (int x = 0; x < resX; x++)
            {
                for (int y = 0; y < resY; y++)
                {
                    Vector2Int posP = screenPos;
                    posP.x += x - _resXH;
                    posP.y += y - _resYH;

                    //If the current "pixel" is out of bounds, skip
                    if (!Renderer.IsInBounds(posP)) { continue; }

                    int index = posP.y * Renderer.WORLD_RES_X + posP.x;
                    int indexC = y * resX + x;

                    int orderOff = order + orderOffset;
                    //If the depth at the current position in graphic space is greater or equal to this sprite's renderig order, skip 
                    if (depth[index] >= (isMasker ? -orderOff : orderOff)) { continue; }

                    //This is just here to make the player's Collider visible in when the player is behind other sprites
                    if (isMasker && state.State[index] == Mask[indexC]) { continue; }
                    char c = Chars[indexC];

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
            sb.AppendLine($"XY: {resX}/{resY}\nOrder: {order}\n");
            for (int y = 0; y < resY; y++)
            {
                for (int x = 0; x < resX; x++)
                {
                    sb.Append(Chars[y * resX + x]);
                }

                sb.Append('\n');
            }
            System.Diagnostics.Debug.Print(sb.ToString());
        }
    }
}