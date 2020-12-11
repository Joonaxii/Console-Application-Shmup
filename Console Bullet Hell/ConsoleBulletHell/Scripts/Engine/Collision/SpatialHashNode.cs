using System;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class SpatialHashNode : IEquatable<SpatialHashNode>
    {
        public List<ICollideable> Bodies;
        public int hash;

        public SpatialHashNode(int _hash)
        {
            hash = _hash;
            Bodies = new List<ICollideable>(512);
        }

        public bool Equals(SpatialHashNode other)
        {
            return other.hash.Equals(hash);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public void Add(ICollideable shape)
        {
            if (shape.HasNode(hash))
            {
                return;
            }

            Bodies.Add(shape);
            shape.AddNode(hash);
        }

        public void Clear()
        {
            Bodies.Clear();
        }
    }
}