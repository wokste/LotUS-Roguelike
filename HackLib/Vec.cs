using System;

namespace HackLib
{
    public struct Vec
    {
        public int X;
        public int Y;

        public Vec(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec operator +(Vec l, Vec r) => new Vec(l.X + r.X, l.Y + r.Y);
        public static Vec operator -(Vec l, Vec r) => new Vec(l.X - r.X, l.Y - r.Y);

        public static Vec operator *(Vec l, int r) => new Vec(l.X * r, l.Y * r);
        public static Vec operator /(Vec l, int r) => new Vec(l.X / r, l.Y / r);

        public static bool operator ==(Vec l, Vec r) => (l.X == r.X && l.Y == r.Y);
        public static bool operator !=(Vec l, Vec r) => (l.X != r.X || l.Y != r.Y);

        public int LengthSquared => X * X + Y * Y;
        public double Length => Math.Sqrt(LengthSquared);

        public double ManhattanLength
        {
            get
            {
                var aX = Math.Abs(X);
                var aY = Math.Abs(Y);
                return (aX < aY) ? aY + aX * 0.5 : aX + aY * 0.5;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is Vec) && (this == (Vec) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }

    public struct Rect
    {
        public int Left, Top, Width, Height;

        public int Bottom => Top + Height;
        public int Right => Left + Width;

        public Rect(Vec pos, Vec size)
        {
            Left = pos.X;
            Top = pos.Y;
            Width = size.X;
            Height = size.Y;
        }
    }
}