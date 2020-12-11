namespace Joonaxii.ConsoleBulletHell
{
    public class Collider
    {
        public ICollideable Host { get; private set; }

        public Vector2 Offset;
        public Vector2 Center;

        public ColliderBase[] CollisionShapes;
        public Rect2D bounds;

        public Collider(ICollideable host, Vector2 offset, ColliderBase[] colliders)
        {
            Host = host;
            Offset = offset;
            CollisionShapes = colliders;
        }

        public void Update(Vector2 entPos)
        {
            Center = entPos + Offset;
            bounds.center = Center;

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            ColliderBase curShape;

            for (int i = 0; i < CollisionShapes.Length; i++)
            {
                curShape = CollisionShapes[i];
                if (curShape.Min.y < min.y)
                {
                    min.y = curShape.Min.y;
                }

                if (curShape.Max.y > max.y)
                {
                    max.y = curShape.Max.y;
                }

                if (curShape.Min.x < min.x)
                {
                    min.x = curShape.Min.x;
                }

                if (curShape.Max.x > max.x)
                {
                    max.x = curShape.Max.x;
                }

                curShape.Update(Center);
            }

            bounds.UpdateFast(min, max);
        }

        public bool CollidesWith(Collider other)
        {
            ColliderBase curA;
            ColliderBase curB;
            bool hit = false;
            for (int i = 0; i < CollisionShapes.Length; i++)
            {
                curA = CollisionShapes[i];
                for (int j = 0; j < other.CollisionShapes.Length; j++)
                {
                    if (hit) { break; }
                    curB = other.CollisionShapes[j];
                    switch (curB.Type)
                    {
                        case ColliderType.POINT:
                            hit |= curA.VSPoint(curB as PointCollider);
                            break;
                        case ColliderType.CIRCLE:
                            hit |= curA.VSCircle(curB as CircleCollider);
                            break;
                        case ColliderType.BOX:
                            hit |= curA.VSBox(curB as BoxCollider);
                            break;
                    }
                }
                if (hit) { break; }
            }

            return hit;
        }
    }
}