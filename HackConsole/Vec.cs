using System;

namespace HackConsole
{
    public struct Vec
    {
        public readonly int X, Y;
        public static Vec Zero = new Vec(0, 0);

        /// <summary>Not a vector. Similar to NaN</summary>
        //public static const Vec NaV = new Vec(int.MinValue, int.MinValue);

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
        public int Area => X * Y;
        public double Length => Math.Sqrt(LengthSquared);

        public float ManhattanLength {
            get {
                var aX = Math.Abs(X);
                var aY = Math.Abs(Y);
                return (aX < aY) ? aY + aX * 0.5f : aX + aY * 0.5f;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is Vec v) && (this == v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    public struct Rect
    {
        public int Left, Top, Width, Height;

        public int Right => Left + Width;
        public int Bottom => Top + Height;

        public Vec TopLeft => new Vec(Left, Top);
        public Vec Size => new Vec(Width, Height);
        public Vec BottomRight => new Vec(Left + Width, Top + Height);
        public Vec Center => new Vec(Width / 2 + Left, Height / 2 + Top);

        public static Rect operator +(Rect l, Vec r) => new Rect(l.TopLeft + r, l.Size);
        public static Rect operator -(Rect l, Vec r) => new Rect(l.TopLeft - r, l.Size);

        public Rect(Vec pos, Vec size)
        {
            Left = pos.X;
            Top = pos.Y;
            Width = size.X;
            Height = size.Y;
        }

        public bool Contains(Vec v)
        {
            return (v.X >= Left) && (v.Y >= Top) && (v.X < Right) && (v.Y < Bottom);
        }
    }
}