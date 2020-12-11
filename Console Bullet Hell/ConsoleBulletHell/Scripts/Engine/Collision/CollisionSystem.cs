using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class CollisionSystem
    {
        public List<Collider> AllCollideables;
        public SpatialHashGrid CollisionGrid;

        public CollisionSystem()
        {
            AllCollideables = new List<Collider>();
            CollisionGrid = new SpatialHashGrid(new Vector2Int(30, 20), 30);
        }

        private ICollideable[] _shapes = new ICollideable[8192];
        public void Update()
        {
            Entity colliderA;
            Entity colliderB;

            //Clear the collision grid and entities, and then reinsert them into the grid after updating
            CollisionGrid.ClearGrid();
            for (int k = 0; k < AllCollideables.Count; k++)
            {
                colliderA = AllCollideables[k].Host.GetEntity();
                colliderA.Nodes.Clear();

                //If the collider can collide, update It's position and boundaries, then insert it to the collision grid
                if (colliderA.CanCollide)
                {
                    colliderA.Collider.Update(colliderA.Position);
                    CollisionGrid.Update(colliderA.Collider);
                }
            }

            //Loop over every ICollideable, retrieve all possible collisions it could have, then run collisions between 'em
            for (int i = 0; i < AllCollideables.Count; i++)
            {
                colliderA = AllCollideables[i].Host.GetEntity();

                //If this collider cannot collide, don't do anything
                if (!colliderA.CanCollide) { continue; }

                int retrieveL = CollisionGrid.Retrieve(ref _shapes, colliderA);
                for (int j = 0; j < retrieveL; j++)
                {
                    colliderB = _shapes[j].GetEntity();

                    bool hasCollision = colliderA.HasCollision(colliderB.ID);
                    bool collision = colliderA.Collider.CollidesWith(colliderB.Collider);

                    //If the colliders aren't colliding check if they were colliding last time, if yes call OnCollisionExit on both
                    //and remove their IDs from each other's collision lists
                    if (!collision)
                    {
                        if (hasCollision)
                        {
                            colliderA.RemoveCollision(colliderB.ID);
                            colliderB.RemoveCollision(colliderA.ID);

                            colliderA.OnCollisionExit(colliderB);
                            colliderB.OnCollisionExit(colliderA);
                        }
                        continue;
                    }

                    //If however the colliders are colliding, check if this collision happened this frame
                    //If yes, call OnCollisionEnter on colliderA and add colliderB's ID to it's collisions list                    
                    if (!hasCollision)
                    {
                        colliderA.OnCollisionEnter(colliderB);
                        colliderA.AddCollision(colliderB.ID);
                    }

                    //OnCollisionStay gets called every frame the colliders are colliding, including the OnEnter frame
                    colliderA.OnCollisionStay(colliderB);
                }
            }
        }

        public static bool CircleVSCircle(Vector2 posA, Vector2 posB, float radA, float radB)
        {
            float radius = radA + radB;

            float deltaX = (posA.x - posB.x);
            float deltaY = (posA.y - posB.y);

            return deltaX * deltaX + deltaY * deltaY <= radius * radius;
        }

        public static bool PointVSCircle(Vector2 posA, Vector2 posB, float radiusSqr)
        {
            float deltaX = (posA.x - posB.x);
            float deltaY = (posA.y - posB.y);

            return deltaX * deltaX + deltaY * deltaY <= radiusSqr;
        }

        public static bool BoxVSCircle(Vector2 posA, float radiusSqr, Vector2 min, Vector2 max)
        {
            float deltaX = posA.x - Maths.Clamp(posA.x, min.x, max.x);
            float deltaY = posA.y - Maths.Clamp(posA.y, min.y, max.y);

            return (deltaX * deltaX) + (deltaY * deltaY) <= radiusSqr;
        }

        public static bool BoxVSBox(Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB)
        {
            return !(minA.x > maxB.x | minA.y > maxB.y | maxA.x < minB.x | maxA.y < minB.y);
        }

        public static bool BoxVSPoint(Vector2 min, Vector2 max, Vector2 point)
        {
            return !(min.x > point.x | min.y > point.y | max.x < point.x | max.y < point.y);
        }
    }
}