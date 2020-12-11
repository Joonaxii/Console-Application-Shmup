namespace Joonaxii.ConsoleBulletHell
{
    public class CircleCollider : ColliderBase
    {
        public float Radius;
        public float RadiusSqrd;

        public CircleCollider(Vector2 offset, float radius) : base(ColliderType.CIRCLE, offset, new Vector2(radius, radius))
        {
            Radius = radius;
            RadiusSqrd = radius * radius;
            SetSprite();
        }

        public override ColliderBase Clone()
        {
            return MemberwiseClone() as CircleCollider;
        }

        public void SetRadius(float radius)
        {
            if(Radius == radius) { return; }

            Radius = radius;
            RadiusSqrd = Radius * Radius;
            SetExtents(new Vector2(radius, radius));
            SetSprite();
        }

        private void SetSprite()
        {
            ShapeSprite = SpriteFactory.GetCircle(Radius, RadiusSqrd);
        }

        public override bool VSBox(BoxCollider col)
        {
            return CollisionSystem.BoxVSCircle(Center, RadiusSqrd, col.Min, col.Max);
        }

        public override bool VSCircle(CircleCollider col)
        {
            return CollisionSystem.CircleVSCircle(Center, col.Center, Radius, col.Radius);
        }

        public override bool VSPoint(PointCollider col)
        {
            return CollisionSystem.PointVSCircle(Center, col.Center, RadiusSqrd);
        }
    }
}