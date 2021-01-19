namespace Joonaxii.ConsoleBulletHell
{
    public class PointCollider : ColliderBase
    {
        public PointCollider(Vector2 offset) : base(ColliderType.POINT, offset, Vector2.zero)
        {
            ShapeSprite = null;
        }

        public override ColliderBase Clone() => MemberwiseClone() as PointCollider;
        public override bool VSBox(BoxCollider col) => CollisionSystem.BoxVSPoint(col.Min, col.Max, Center);
        public override bool VSCircle(CircleCollider col)
        {
            return CollisionSystem.PointVSCircle(Center, col.Center, col.Radius);
        }

        public override bool VSPoint(PointCollider col)
        {
            return col.Center == Center;
        }
    }
}