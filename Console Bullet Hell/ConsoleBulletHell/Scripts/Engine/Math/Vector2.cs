using System;
using System.Runtime.InteropServices;

namespace Joonaxii.ConsoleBulletHell
{
    [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 4)]
    public struct Vector2 : IEquatable<Vector2>
    {
        public static Vector2 zero = new Vector2(0, 0);
        public static Vector2 up = new Vector2(0, 1.0f);
        public static Vector2 one = new Vector2(1.0f, 1.0f);
        public static Vector2 right = new Vector2(1.0f, 0.0f);

        public float x;
        public float y;

        public float Magnitude => (float)Math.Sqrt(SqrMagnitude);
        public Vector2 Normalized => new Vector2(x, y).Normalize();
        public float SqrMagnitude => x * x + y * y;

        public Vector2(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public void Set(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        public static float Dot(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.x + lhs.y * rhs.y;

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float deltaTime, float maxSpeed = float.MaxValue)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);

            float stHalf = smoothTime * 0.5f;
            float stDelta = stHalf * deltaTime;

            float d = 1.0f / (1.0f + stDelta + 0.48f * stDelta * stDelta + 0.235f * stDelta * stDelta * stDelta);
            Vector2 diff = current - target;
            Vector2 tgt = target;

            float maxLength = maxSpeed * smoothTime;
            diff = ClampMagnitude(diff, maxLength);
            target = current - diff;

            Vector2 velDiff = (currentVelocity + diff * stHalf) * deltaTime;
            currentVelocity = (currentVelocity - velDiff * stHalf) * d;

            Vector2 tgtDiff = target + (diff + velDiff) * d;
            if (Dot(tgt - current, tgtDiff - tgt) > 0f)
            {
                tgtDiff = tgt;
                currentVelocity = (tgtDiff - tgt) / deltaTime;
            }
            return tgtDiff;
        }

        public bool Equals(Vector2 other) => x == other.x && y == other.y;

        public override bool Equals(object obj) => obj is Vector2 && Equals((Vector2) obj);
        public override string ToString() => $"({x}, {y})";

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public Vector2 Normalize()
        {
            float magnitude = Magnitude;
            return (this = magnitude > 1E-05f ? this / magnitude : zero);
        }

        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            return vector.SqrMagnitude > maxLength * maxLength ? vector.Normalized * maxLength : vector;
        }

        public static explicit operator Vector2Int(Vector2 vector) => new Vector2Int(Maths.RoundToInt(vector.x), Maths.RoundToInt(vector.y));
        public static explicit operator Vector3Int(Vector2 vector) => new Vector3Int(Maths.RoundToInt(vector.x), Maths.RoundToInt(vector.y), 0);

        public static implicit operator Vector2(Vector2Int vector) => new Vector2(vector.x, vector.y);
        public static implicit operator Vector2(Vector3Int vector) => new Vector2(vector.x, vector.y);

        public static bool operator ==(Vector2 vector1, Vector2 vector2) => vector1.Equals(vector2);
        public static bool operator !=(Vector2 vector1, Vector2 vector2) => !(vector1 == vector2);

        public static Vector2 operator *(Vector2 vector1, float val) => new Vector2(vector1.x * val, vector1.y * val);
        public static Vector2 operator /(Vector2 vector1, float val) => new Vector2(vector1.x / val, vector1.y / val);
  
        public static Vector2 operator *(float val, Vector2 vector1) => new Vector2(vector1.x * val, vector1.y * val);
        public static Vector2 operator /(float val, Vector2 vector1) => new Vector2(vector1.x / val, vector1.y / val);

        public static Vector2 operator *(Vector2 vector1, Vector2 vector2) => new Vector2(vector1.x* vector2.x, vector1.y* vector2.y);
        public static Vector2 operator /(Vector2 vector1, Vector2 vector2) => new Vector2(vector1.x / vector2.x, vector1.y / vector2.y);

        public static Vector2 operator +(Vector2 vector1, Vector2 vector2) => new Vector2(vector1.x + vector2.x, vector1.y + vector2.y);
        public static Vector2 operator -(Vector2 vector1, Vector2 vector2) => new Vector2(vector1.x - vector2.x, vector1.y - vector2.y);

        public static Vector2 operator -(Vector2 vector1) => new Vector2(-vector1.x, -vector1.y);
    }
}