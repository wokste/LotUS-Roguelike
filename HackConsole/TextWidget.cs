using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole
{
    public abstract class TextWidget : Widget , IInputReader
    {
        protected readonly List<String> _lines = new List<string>();
        protected bool _dirty = true;
        protected int posY;

        public override void Render(bool forceUpdate)
        {
            if (!forceUpdate && !_dirty)
                return;

            _dirty = false;

            Clear();

            var y = 0;

            var firstLine = posY;
            for (var i = firstLine; i < Math.Min(firstLine + Size.Height, _lines.Count); i++)
            {
                Print(0, y, _lines[i], Color.Yellow);
                y++;
            }
        }

        protected void WordWrap(string msg, string prefix)
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
                    _lines.Add((lineStart == 0 ? prefix : noPrefix) + msg.Substring(lineStart, lastSpace - lineStart));

                    lineStart = lastSpace + 1;
                }
                if (msg[i] == ' ')
                    lastSpace = i;
            }
            _lines.Add((lineStart == 0 ? prefix : noPrefix) + msg.Substring(lineStart, msg.Length - lineStart));
        }

        protected override void OnResized()
        {
            // If the width has changed, the lines need to be recalculated.
            MakeLines();
            posY = Math.Max(0, _lines.Count - Size.Height);
            _dirty = true;
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
            _dirty = true;
            posY = MyMath.Clamp(posY - delta.Y, 0, _lines.Count - 1);

            return true;
        }
    }
}
