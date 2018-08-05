using HackConsole.Algo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HackConsole
{
    public class MessageListWidget : GridWidget, IMouseEventSuscriber
    {
        private readonly List<ColoredString> _messages = new List<ColoredString>();
        protected readonly List<string> Lines = new List<string>();

        private int _posY;

        protected int PosY {
            get => _posY;
            set {
                var max = Math.Max(0, Lines.Count - Rect.Height);
                _posY = MyMath.Clamp(value, 0, max);
            }
        }

        protected override void Render()
        {
            Clear();

            var y = 0;

            var firstLine = _posY;
            for (var i = firstLine; i < Math.Min(firstLine + Rect.Height, Lines.Count); i++)
            {
                Print(new Vec(0, y), Lines[i], Color.White);
                y++;
            }
        }

        public void Add(ColoredString msg)
        {
            _messages.Add(msg);
            var range = StringExt.Prefix(StringExt.Wrap(msg.Text, Rect.Width - 2), "> ");
            PosY += range.Count();
            Lines.AddRange(range);
            Dirty = true;
        }

        protected void MakeLines()
        {
            Lines.Clear();
            foreach (var msg in _messages)
                Lines.AddRange(StringExt.Prefix(StringExt.Wrap(msg.Text, Rect.Width - 2), "> "));
        }

        protected override void OnResized()
        {
            base.OnResized();
            // If the width has changed, the lines need to be recalculated.
            MakeLines();
            PosY = Math.Max(0, Lines.Count - Rect.Height);
            Dirty = true;
        }

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
