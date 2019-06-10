using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace HackConsole
{
    public abstract class TextWidget : GridWidget , IMouseEventSuscriber
    {
        protected readonly List<string> Lines = new List<string>();

        private int _posY;

        protected int PosY
        {
            get => _posY;
            set
            {
                var max = Math.Max(0, Lines.Count - Rect.Height);
                _posY = MyMath.Clamp(value, 0, max);
            }
        }

        protected override void Render()
        {
            Clear(new Color(128,128,128));

            var y = 0;

            var firstLine = _posY;
            for (var i = firstLine; i < Math.Min(firstLine + Rect.Height, Lines.Count); i++)
            {
                Print(new Vec(0, y), Lines[i], Color.White);
                y++;
            }
        }

        protected override void OnResized()
        {
            base.OnResized();
            // If the width has changed, the lines need to be recalculated.
            MakeLines();
            PosY = Math.Max(0, Lines.Count - Rect.Height);
            Dirty = true;
        }

        protected abstract void MakeLines();

        public virtual void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
        }

        public virtual void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
            Dirty = true;
            PosY -= delta.Y;
        }
    }
}
