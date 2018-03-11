using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole
{
    public abstract class TextWidget : Widget , IInputReader
    {
        protected readonly List<(string,Color)> Lines = new List<(string, Color)>();
        protected bool Dirty = true;
        protected int PosY;

        public override void Render(bool forceUpdate)
        {
            if (!forceUpdate && !Dirty)
                return;

            Dirty = false;

            Clear();

            var y = 0;

            var firstLine = PosY;
            for (var i = firstLine; i < Math.Min(firstLine + Size.Height, Lines.Count); i++)
            {
                Print(0, y, Lines[i].Item1, Lines[i].Item2);
                y++;
            }
        }

        protected void WordWrap(string msg, string prefix, Color color)
        {
            var noPrefix = new string(' ', prefix.Length);

            var lineStart = 0;
            var lastSpace = 0;

            var maxWidth = Size.Width - prefix.Length;
            if (maxWidth < 1)
                return;

            for (var i = 0; i < msg.Length; i++)
            {
                if (i - lineStart > maxWidth)
                {
                    var txt = (lineStart == 0 ? prefix : noPrefix) + msg.Substring(lineStart, lastSpace - lineStart);
                    Lines.Add((txt,color));

                    lineStart = lastSpace + 1;
                }
                if (msg[i] == ' ')
                    lastSpace = i;
            }
            {
                var txt = (lineStart == 0 ? prefix : noPrefix) + msg.Substring(lineStart, msg.Length - lineStart);
                Lines.Add((txt, color));
            }
        }

        protected override void OnResized()
        {
            // If the width has changed, the lines need to be recalculated.
            MakeLines();
            PosY = Math.Max(0, Lines.Count - Size.Height);
            Dirty = true;
        }

        protected abstract void MakeLines();

        public bool OnKeyPress(char keyCode, EventFlags flags)
        {
            return true;
        }

        public bool OnArrowPress(Vec move, EventFlags flags)
        {
            return true;
        }

        public bool OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            return true;
        }

        public bool OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            return true;
        }

        public bool OnMouseWheel(Vec delta, EventFlags flags)
        {
            Dirty = true;
            PosY = MyMath.Clamp(PosY - delta.Y, 0, Lines.Count - 1);

            return true;
        }
    }
}
