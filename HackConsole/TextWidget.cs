using System;
using System.Collections.Generic;

namespace HackConsole
{
    public abstract class TextWidget : Widget , IMouseEventSuscriber
    {
        protected readonly List<(string,Color)> Lines = new List<(string, Color)>();
        protected bool Dirty = true;

        private int _posY;

        protected int PosY
        {
            get => _posY;
            set
            {
                var max = Math.Max(0, Lines.Count - Size.Height);
                _posY = MyMath.Clamp(value, 0, max);
            }
        }

        public override void Render(bool forceUpdate)
        {
            if (!forceUpdate && !Dirty)
                return;

            Dirty = false;

            Clear();

            var y = 0;

            var firstLine = _posY;
            for (var i = firstLine; i < Math.Min(firstLine + Size.Height, Lines.Count); i++)
            {
                Print(0, y, Lines[i].Item1, Lines[i].Item2);
                y++;
            }
        }

        protected int WordWrap(string msg, string prefix, Color color)
        {
            var count = 0;
            var noPrefix = new string(' ', prefix.Length);

            var lineStart = 0;
            var lastSpace = 0;

            var maxWidth = Size.Width - prefix.Length;
            if (maxWidth < 1)
                return 0;

            for (var i = 0; i < msg.Length; i++)
            {
                if (i - lineStart > maxWidth)
                {
                    var txt = (lineStart == 0 ? prefix : noPrefix) + msg.Substring(lineStart, lastSpace - lineStart);
                    Lines.Add((txt,color));
                    count++;

                    lineStart = lastSpace + 1;
                }
                if (msg[i] == ' ')
                    lastSpace = i;
            }
            {
                var txt = (lineStart == 0 ? prefix : noPrefix) + msg.Substring(lineStart, msg.Length - lineStart);
                Lines.Add((txt, color));
                count++;
            }
            return count;
        }

        protected override void OnResized()
        {
            // If the width has changed, the lines need to be recalculated.
            MakeLines();
            PosY = Math.Max(0, Lines.Count - Size.Height);
            Dirty = true;
        }

        protected abstract void MakeLines();

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
            Dirty = true;
            PosY -= delta.Y;
        }
    }
}
