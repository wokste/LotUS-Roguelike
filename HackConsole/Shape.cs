using System;
using System.Collections.Generic;

namespace HackConsole
{
    public interface IShape
    {
        Rect BoundingBox { get; }
        bool Contains(Vec v);
        IEnumerable<Vec> Iterator();
    }

    public struct Circle : IShape
    {
        public Vec Center;
        public readonly double Radius;
        private readonly int _radiusInt;

        public Circle(Vec center, double radius = 0) : this()
        {
            Center = center;
            Radius = radius;
            _radiusInt = (int)Math.Ceiling(Radius);
        }

        public Rect BoundingBox => new Rect(Center.X - _radiusInt, Center.Y - _radiusInt, 2 * _radiusInt + 1, 2 * _radiusInt + 1);

        public bool Contains(Vec v)
        {
            return ((Center - v).LengthSquared <= Radius * Radius);
        }

        public IEnumerable<Vec> Iterator()
        {
            foreach (var v in BoundingBox.Iterator())
                if (Contains(v))
                    yield return v;
        }
    }

    public struct Rect : IShape
    {
        public int Left, Top, Width, Height;

        public int Right => Left + Width;
        public int Bottom => Top + Height;

        public Vec TopLeft => new Vec(Left, Top);
        public Size Size => new Size(Width, Height);
        public Vec BottomRight => new Vec(Left + Width, Top + Height);
        public Vec Center => new Vec(Width / 2 + Left, Height / 2 + Top);

        public Rect BoundingBox => this;

        public static Rect operator +(Rect l, Vec r) => new Rect(l.TopLeft + r, l.Size);
        public static Rect operator -(Rect l, Vec r) => new Rect(l.TopLeft - r, l.Size);

        public Rect(Vec pos, Size size)
        {
            Left = pos.X;
            Top = pos.Y;
            Width = size.X;
            Height = size.Y;
        }

        public Rect(int x, int y, int w, int h)
        {
            Left = x;
            Top = y;
            Width = w;
            Height = h;
        }

        public bool Contains(Vec v)
        {
            return (v.X >= Left) && (v.Y >= Top) && (v.X < Right) && (v.Y < Bottom);
        }

        public IEnumerable<Vec> Iterator()
        {
            for (var y = Top; y < Bottom; ++y)
                for (var x = Left; x < Right; ++x)
                    yield return new Vec(x, y);
        }

        public Rect Intersect(Rect other)
        {
            return new Rect
            {
                Left = Math.Max(Left, other.Left),
                Top = Math.Max(Top, other.Top),
                Width = Math.Min(Right, other.Right) - Math.Max(Left, other.Left),
                Height = Math.Min(Bottom, other.Bottom) - Math.Max(Top, other.Top),
            };
        }

        public Rect Grow(int count)
        {
            return new Rect
            {
                Left = Left - count,
                Top = Top - count,
                Width = Width + 2 * count,
                Height = Height + 2 * count,
            };
        }
    }
}
