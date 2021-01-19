using System;

namespace Joonaxii.ConsoleBulletHell
{
    /// <summary>
    /// A collection of various Math methods and constants
    /// </summary>
    public static class Maths
    {
        public const float PI = (float)Math.PI;
        public const float TWO_PI = PI * 2.0f;
        public const float Epsilon = 1E-15f;
        public const float Deg2Rad = PI / 180;
        public const float Rad2Deg = 360.0f / TWO_PI;

        #region Math Logic

        public static bool InRange(this int input, int min, int max) => input >= min & input <= max;

        public static float Clamp(float input, float min, float max) => input < min ? min : input > max ? max : input;
        public static int Clamp(int input, int min, int max) => input < min ? min : input > max ? max : input;

        public static float Sign(float input) => input < 0 ? -1 : 1;

        public static int CeilToInt(float f) => (int)Math.Ceiling(f);
        public static int RoundToInt(float f) => (int)Math.Round(f);
        public static int FloorToInt(float f) => (int)Math.Floor(f);

        public static float Repeat(float t, float length) => Clamp(t - (float)Math.Floor(t / length) * length, 0f, length);

        #region Comparison

        public static bool CompareTo(this float input, float other, ComparisonType type)
        {
            switch (type)
            {
                default:
                    return input == other;
                case ComparisonType.NOT_EQUAL:
                    return input != other;

                case ComparisonType.EQUAL_ABS:
                    return input == Math.Abs(other);
                case ComparisonType.NOT_EQUAL_ABS:
                    return input != Math.Abs(other);


                case ComparisonType.GREATER_OR_EQUAL:
                    return input >= other;
                case ComparisonType.GREATER_OR_EQUAL_ABS:
                    return input >= Math.Abs(other);

                case ComparisonType.GREATER_THAN:
                    return input > other;
                case ComparisonType.GREATER_THAN_ABS:
                    return input > Math.Abs(other);


                case ComparisonType.LESS_OR_EQUAL:
                    return input <= other;
                case ComparisonType.LESS_OR_EQUAL_ABS:
                    return input <= Math.Abs(other);

                case ComparisonType.LESS_THAN:
                    return input < other;
                case ComparisonType.LESS_THAN_ABS:
                    return input < Math.Abs(other);
            }

            public static bool CompareTo(this int input, int other, ComparisonType type)
            {
                switch (type)
                {
                    default:
                        return input == other;
                    case ComparisonType.NOT_EQUAL:
                        return input != other;

                    case ComparisonType.EQUAL_ABS:
                        return input == Math.Abs(other);
                    case ComparisonType.NOT_EQUAL_ABS:
                        return input != Math.Abs(other);


                    case ComparisonType.GREATER_OR_EQUAL:
                        return input >= other;
                    case ComparisonType.GREATER_OR_EQUAL_ABS:
                        return input >= Math.Abs(other);

                    case ComparisonType.GREATER_THAN:
                        return input > other;
                    case ComparisonType.GREATER_THAN_ABS:
                        return input > Math.Abs(other);


                    case ComparisonType.LESS_OR_EQUAL:
                        return input <= other;
                    case ComparisonType.LESS_OR_EQUAL_ABS:
                        return input <= Math.Abs(other);

                    case ComparisonType.LESS_THAN:
                        return input < other;
                    case ComparisonType.LESS_THAN_ABS:
                        return input < Math.Abs(other);
                }
            }
        }

        #endregion

        #endregion

        #region Wave Functions
        public static float Sin(float f) => (float)Math.Sin(f);
        public static float Cos(float f) => (float)Math.Cos(f);

        public static float Square(float t, bool sine = true) => Math.Sign(sine ? Sin(TWO_PI * t) : Cos(TWO_PI * t));
        public static float Triangle(float t) => 1f - 4f * (float)Math.Abs(Math.Round(t - 0.25f) - (t - 0.25f));
        public static float Sawtooth(float t) => 2f * (t - (float)Math.Floor(t + 0.5f));
        #endregion

        #region Vector Math

        #region Vector Rotation
        public static float Angle(this Vector2 a, Vector2 b)
        {
            float sqr = (float)Math.Sqrt((a.SqrMagnitude * b.SqrMagnitude));
            return sqr < Epsilon ? 0.0f : (float)Math.Acos(Clamp(Vector2.Dot(a, b) / sqr, -1f, 1f)) * Rad2Deg;
        }

        public static float SignedAngle(this Vector2 a, Vector2 b) => Angle(a, b) * Sign(a.x* b.y - a.y* b.x);

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float rads = degrees * Deg2Rad;

            float sin = Sin(rads);
            float cos = Cos(rads);

            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
        #endregion

        public static Vector2 QuadraticBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;

            return uu * p0 + (2.0f * u * t) * p1 + tt * p2;
        }
        public static Vector2 CubicBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1.0f - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * t;

            return u3 * p0 +
                (3.0f * u2 * t) * p1 +
                (3.0f * u * t2) * p2 +
                t3 * p3;
        }

        #region Distance

        public static float SqrDistance(float x1, float x2, float y1, float y2)
        {
            float x = x1 - x2;
            float y = y1 - y2;

            return (x * x) + (y * y);
        }

        public static float SqrDistance(Vector2 a, Vector2 b) => SqrDistance(a.x, b.x, a.y, b.y);
        public static float Distance(Vector2 a, Vector2 b) => (float)Math.Sqrt(SqrDistance(a, b));


        #endregion
        #endregion

        #region Interpolation

        public static float SmoothStep(float from, float to, float t)
        {
            t = t > 1.0f ? 1.0f : t < 0 ? 0 : t;
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        public static float InverseLerp(float a, float b, float value) => a != b ? (value - a) / (b - a) : 0f;
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static float EaseIn(float t) => t * t;
        public static float EaseOut(float t) => t * (2f - t);

        #endregion
    }
}