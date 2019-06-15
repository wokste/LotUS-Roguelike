using System;
using System.Collections.Generic;

namespace HackConsole
{
    public struct Vec : IShape
    {
        public readonly int X, Y;
        public static Vec Zero = new Vec(0, 0);
        public static Vec One = new Vec(1, 1);

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

        public int ManhattanLength {
            get {
                var aX = Math.Abs(X);
                var aY = Math.Abs(Y);
                return Math.Max(aX, aY);
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
                return ((X * 397) ^ Y) * 1389;
            }
        }

        public override string ToString() => $"({X},{Y})";

        // Shape
        public Rect BoundingBox => new Rect(this, Size.One);

        public Vec Clamped => new Vec(MyMath.Clamp(X, -1, 1), MyMath.Clamp(Y, -1, 1));

        public bool Contains(Vec v) => v == this;
        public IEnumerable<Vec> Iterator() { yield return this; }
    }

    public struct Size : IShape
    {
        public readonly int X, Y;
        public static Size Zero = new Size(0, 0);
        public static Size One = new Size(1, 1);

        public Size(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Size operator +(Size l, Vec r) => new Size(l.X + r.X, l.Y + r.Y);
        public static Size operator -(Size l, Vec r) => new Size(l.X - r.X, l.Y - r.Y);
        public static Vec operator -(Size l, Size r) => new Vec(l.X - r.X, l.Y - r.Y);

        public static bool operator ==(Size l, Size r) => (l.X == r.X && l.Y == r.Y);
        public static bool operator !=(Size l, Size r) => (l.X != r.X || l.Y != r.Y);

        public int Area => X * Y;

        public override bool Equals(object obj)
        {
            return (obj is Size v) && (this == v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public bool Contains(Vec v)
        {
            return (v.X >= 0) && (v.Y >= 0) && (v.X < X) && (v.Y < Y);
        }

        public IEnumerable<Vec> Iterator()
        {
            for (var y = 0; y < Y; ++y)
                for (var x = 0; x < X; ++x)
                    yield return new Vec(x, y);
        }

        public Rect ToRect() => new Rect(Vec.Zero, this);
        public Vec ToVec() => new Vec(X, Y);
        public Vec BottomRight => new Vec(X, Y);
        public Vec TopLeft => new Vec(0, 0);
        public Vec Center => new Vec(X/2, Y/2);

        public Rect BoundingBox => this.ToRect();
    }
}