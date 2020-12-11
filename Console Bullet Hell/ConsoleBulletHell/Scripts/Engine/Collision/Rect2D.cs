namespace Joonaxii.ConsoleBulletHell
{
    public struct Rect2D
    {
        public Vector2 Size;
        public Vector2 Extents;

        public Vector2 center;

        public Vector2 min, max;

        public static Rect2D zero = new Rect2D(0, 0, 0, 0);

        public Rect2D(float x, float y, float width, float height)
        {
            min.x = x;
            min.y = y;

            max.x = min.x + width;
            max.y = min.y + height;

            Size.x = width;
            Size.y = height;

            Extents = Size * 0.5f;

            center.x = max.x - Extents.x;
            center.y = max.y - Extents.y;

        }

        public Rect2D(Vector2 pos, float width, float height)
        {
            min.x = pos.x - (width * 0.5f);
            min.y = pos.y - (height * 0.5f);

            max.x = min.x + width;
            max.y = min.y + height;

            Size.x = width;
            Size.y = height;

            Extents = Size * 0.5f;

            center = pos;
        }

        public void SetCircle(float x, float y, float width, float height)
        {
            min.x = x;
            min.y = y;

            max.x = min.x + width;
            max.y = min.y + height;

            Size.x = width * 2.0f;
            Size.y = height * 2.0f;

            Extents = Size;

            center.x = min.x - Extents.x;
            center.y = min.y - Extents.y;
        }

        public void Update(float x, float y, float width, float height)
        {
            min.x = x;
            min.y = y;

            max.x = min.x + width;
            max.y = min.y + height;

            Size.x = width;
            Size.y = height;

            Extents = Size * 0.5f;

            center.x = min.x - Extents.x;
            center.y = min.y - Extents.y;
        }

        public void UpdateFast(float x, float y, float width, float height)
        {
            min.x = x;
            min.y = y;

            max.x = min.x + width;
            max.y = min.y + height;

        }

        public void UpdateFast(Vector2 _min, Vector2 _max)
        {
            min = _min;
            max = _max;
        }

        public bool Overlaps(Rect2D other)
        {
            return other.max.x > min.x && other.min.x < max.x && other.max.y > min.y && other.min.y < max.y;
        }

        public override string ToString()
        {
            return string.Format("Center: {0} || Extents: {1} || Size: {2} || Min/Max: {3}/{4}", center.ToString(), Extents.ToString(), Size.ToString(), min.ToString(), max.ToString());
        }
    }
}