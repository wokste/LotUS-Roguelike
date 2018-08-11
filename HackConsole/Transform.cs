using System;
using SFML.Graphics;

namespace HackConsole
{
    public static class TransformExt
    {
        public static Vec TransformVec(this Transform t, Vec v)
        {
            var vf = t.TransformPoint(v.X, v.Y);
            return new Vec((int)Math.Round(vf.X), (int)Math.Round(vf.Y));
        }

        public static Rect TransformRect(this Transform t, Rect r)
        {
            var rf = t.TransformRect(new FloatRect(r.Left, r.Top, r.Width, r.Height));
            // TODO: What should be rounded, width/height or right/bottom
            return new Rect((int)Math.Round(rf.Left), (int)Math.Round(rf.Top), (int)Math.Round(rf.Width), (int)Math.Round(rf.Height));
        }

        public static void Translate(this Transform t, Vec v)
        {
            t.Translate(v.X, v.Y);
        }

        public static Transform MakeTranslateRotate(Vec v, float f)
        {
            var t = Transform.Identity;
            t.Rotate(f);
            t.Translate(v.X, v.Y);
            return t;
        }
    }
}
