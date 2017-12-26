using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool operator ==(Vec l, Vec r) => (l.X == r.X && l.Y == r.Y);
        public static bool operator !=(Vec l, Vec r) => (l.X != r.X || l.Y != r.Y);

        public int LengthSquared => X * X + Y * Y;
        public double Length => Math.Sqrt(LengthSquared);


        public override bool Equals(object obj)
        {
            return (obj is Vec) && (this == (Vec)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}
