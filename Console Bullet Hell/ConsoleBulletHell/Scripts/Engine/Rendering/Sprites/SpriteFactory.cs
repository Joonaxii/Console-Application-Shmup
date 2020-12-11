using System;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public static class SpriteFactory
    {
        public const int DEBUG_LAYER = 9999999;

        private static Dictionary<Vector2Int, Sprite> _boxSprites;
        private static Dictionary<int, Sprite> _circleSprites;

        private static char[] _tileRules = new char[16]
            //{
            //    Renderer.EMPTY_PIXEL_CHAR, //0
            //    '┃', //1
            //    '━', //2
            //    '┏', //3
            //    '┃', //4
            //    '┃', //5
            //    '┗', //6
            //    '┣', //7
            //    '━', //8
            //    '┓', //9
            //    '━', //10
            //    '┳', //11
            //    '┛', //12
            //    '┫', //13
            //    '┻', //14
            //     Renderer.EMPTY_PIXEL_CHAR, //15
            //};
         {
            Renderer.EMPTY_PIXEL_CHAR, //0
            '┃', //1
            '━', //2
            '╱', //3
            '┃', //4
            '┃', //5
            '╲', //6
            '┃', //7
            '━', //8
            '╲', //9
            '━', //10
            '━', //11
            '╱', //12
            '┃', //13
            '━', //14
            Renderer.EMPTY_PIXEL_CHAR, //15
        };


    static SpriteFactory()
        {
            _boxSprites = new Dictionary<Vector2Int, Sprite>();
            _circleSprites = new Dictionary<int, Sprite>();
        }

        public static Sprite GetBox(int width, int height)
        {
            if(_boxSprites.TryGetValue(new Vector2Int(width, height), out Sprite sprt)) { return sprt; }
            
            char[] pixels = new char[width * height];

            int heightOne = height - 1;
            int widthOne = width - 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;
                    if(y == 0)
                    {
                        pixels[i] = x == 0 ? '┏' : x == width - 1 ? '┓' : '━';
                        continue;
                    }

                    if (y == heightOne)
                    {
                        pixels[i] = x == 0 ? '┗' : x == width - 1 ? '┛' : '━';
                        continue;
                    }

                    pixels[i] = x == widthOne | x == 0 ? '┃' : Renderer.EMPTY_PIXEL_CHAR;
                }
            }

            sprt = new Sprite(width, height, pixels, DEBUG_LAYER, 0, 0);
            _boxSprites.Add(new Vector2Int(width, height), sprt);
            return sprt;
        }

        public static Sprite GetCircle(float radius, float radSqrd)
        {
            int reso = (int)Math.Ceiling(radius * 2);
            if (reso < 2 | reso == 3) { return null; }

            if (_circleSprites.TryGetValue(reso, out Sprite sprt)) { return sprt; }

            int resoX = reso * 2;
            int resoY = reso;

            //Add Buffer Room around circle
            int resoAdd = reso + 2;

            char[] pixels = new char[resoAdd * resoAdd];
            byte[] map = new byte[pixels.Length];

            float center = resoAdd * 0.5f;
            float rad = radSqrd;
            for (int y = 0; y < resoAdd; y++)
            {
                for (int x = 0; x < resoAdd; x++)
                {
                    int i = y * resoAdd + x;

                    float dist = Maths.Distance(x - center, -0.5f, y - center, -0.5f);
                    bool inside = dist <= rad;

                    map[i] = (byte)(inside ? 1 : 0);
                }
            }

            for (int y = 0; y < resoAdd; y++)
            {
                for (int x = 0; x < resoAdd; x++)
                {
                    int i = y * resoAdd + x;
                    if(map[i] == 0) { pixels[i] = Renderer.EMPTY_PIXEL_CHAR; continue; }
                    pixels[i] = _tileRules[GetKey(map, resoAdd, resoAdd, x, y)];
                }
            }

            sprt = new Sprite(resoAdd, resoAdd, pixels, DEBUG_LAYER, 0, 0);
            _circleSprites.Add(reso, sprt);
            return sprt;
        }

        private static Vector2Int[] _neighbors = new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        private static byte GetKey(byte[] tile, int w, int h, int x, int y)
        {
            int wOne = w - 1;
            int hOne = h - 1;

            int key = 0;

            Vector2Int neighborOffset;
            for (int i = 0; i < _neighbors.Length; i++)
            {
                neighborOffset = _neighbors[i];
                if (neighborOffset.x == 0 & neighborOffset.y == 0) { continue; }

                int xx = x + neighborOffset.x;
                int yy = y + neighborOffset.y;

                if(xx > wOne | yy > hOne | xx < 0 | yy < 0) { continue; }

                int ii = yy * w + xx;
                if(tile[ii] == 0) { continue; }

                key += 1 << i;
            }

            return (byte)key;
        }
    }
}