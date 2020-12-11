namespace Joonaxii.ConsoleBulletHell
{
    public abstract class ColliderBase
    {
        public ColliderType Type;

        public Vector2 Center;
        public Vector2 Offset;

        public Vector2 Min;
        public Vector2 Max;

        public Sprite ShapeSprite;
  
        private Vector2 _extents;

        public ColliderBase(ColliderType type, Vector2 offset, Vector2 size)
        {
            Type = type;
            Offset = offset;
            SetExtents(size);
        }

        protected void SetExtents(Vector2 size)
        {
            _extents = Type == ColliderType.BOX ? size * 0.5f : size * 2.0f;
            ShapeSprite = SpriteFactory.GetBox((int)size.x, (int)size.y);
        }

        public void Update(Vector2 pos)
        {
            Center = pos + Offset;

            Min = Center - _extents;
            Max = Center + _extents;
        }

        public abstract bool VSPoint(PointCollider col);
        public abstract bool VSBox(BoxCollider col);
        public abstract bool VSCircle(CircleCollider col);

        public abstract ColliderBase Clone();
    }
}