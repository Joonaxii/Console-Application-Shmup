using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Joonaxii.ConsoleBulletHell
{
    [StructLayout(LayoutKind.Sequential, Size = 4, Pack = 1)]
    public struct FastColor : IEquatable<FastColor>
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public FastColor(byte red, byte grn, byte blu, byte alp)
        {
            r = red;
            g = grn;
            b = blu;
            a = alp;
        }

        public char ToChar() => char.ConvertFromUtf32(GetHashCode())[0];

        public override string ToString() => $"{r},{g},{b},{a}";
        public override bool Equals(object obj) => obj is FastColor && Equals((FastColor)obj);
        public override int GetHashCode() => r + (g * 256) + (b * 65536) + (a * 16777216);

        public bool Equals(FastColor other) => (r == other.r & g == other.g & b == other.b & a == other.a); 

        public static FastColor FromHash(int hash) => new FastColor((byte)hash, (byte)(hash >> 8), (byte)(hash >> 16), (byte)(hash >> 24));

        public static implicit operator FastColor(Color color) => new FastColor(color.R, color.G, color.B, color.A);
        public static implicit operator Color(FastColor color) => Color.FromArgb(color.a, color.r, color.g, color.b);

        public static bool operator ==(FastColor color1, FastColor color2) => color1.Equals(color2); 
        public static bool operator !=(FastColor color1, FastColor color2) => !(color1 == color2); 
    }
}