using System;
using System.Runtime.InteropServices;

namespace Joonaxii.ConsoleBulletHell
{
    [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 4)]
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        public int x;
        public int y;

        public Vector2Int(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Int && Equals((Vector2Int)obj);
        }

        public bool Equals(Vector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override string ToString()
        {
            return $"XY: {x}, {y}";
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public void Set(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Vector2Int vector1, Vector2Int vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector2Int vector1, Vector2Int vector2)
        {
            return !(vector1 == vector2);
        }
    }
}