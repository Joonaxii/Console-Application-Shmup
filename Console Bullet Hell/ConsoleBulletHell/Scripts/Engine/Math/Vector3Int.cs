using System;
using System.Runtime.InteropServices;

namespace Joonaxii.ConsoleBulletHell
{
    [StructLayout(LayoutKind.Sequential, Size = 12, Pack = 4)]
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        public int x;
        public int y;
        public int z;

        public Vector3Int(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public void Set(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public bool Equals(Vector3Int other) => x == other.x & y == other.y & z == other.z;
        public override bool Equals(object obj) => obj is Vector3Int && Equals((Vector3Int)obj);

        public override string ToString() => $"XYZ: ({x}, {y}, {z})";

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Vector3Int int1, Vector3Int int2) => int1.Equals(int2);
        public static bool operator !=(Vector3Int int1, Vector3Int int2) => !(int1 == int2);
    }
}