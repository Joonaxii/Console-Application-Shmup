namespace Joonaxii.ConsoleBulletHell
{
    public class BoxCollider : ColliderBase
    {
        public BoxCollider(Vector2 offset, Vector2 size) : base(ColliderType.BOX, offset, size) {}

        public override ColliderBase Clone() => MemberwiseClone() as BoxCollider;

        public override bool VSBox(BoxCollider col)         => CollisionSystem.BoxVSBox(Min, Max, col.Min, col.Max);
        public override bool VSCircle(CircleCollider col)   => CollisionSystem.BoxVSCircle(col.Center, col.RadiusSqrd, Min, Max);
        public override bool VSPoint(PointCollider col)     => CollisionSystem.BoxVSPoint(Min, Max, col.Center);
    }
}