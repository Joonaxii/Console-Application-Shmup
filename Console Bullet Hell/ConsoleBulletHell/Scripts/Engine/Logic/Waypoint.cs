namespace Joonaxii.ConsoleBulletHell
{
    public struct Waypoint
    {
        public Vector2 position;
        public bool relative;

        private Vector2 _actualPosition;

        public Waypoint(Vector2 pos, bool isRelative)
        {
            position = pos;
            relative = isRelative;
            _actualPosition = Vector2.zero;
        }

        public void BakePosition(Vector2 pos)
        {
            _actualPosition = relative ? pos + position : position;
        }

        public Vector2 GetPosition()
        {
            return _actualPosition;
        }
    }
}