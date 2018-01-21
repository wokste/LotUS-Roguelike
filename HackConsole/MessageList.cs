using System;
using System.Collections.Generic;

namespace HackConsole
{
    public class MessageList : Widget
    {
        private readonly List<String> _messages = new List<string>();
        private readonly List<String> _lines = new List<string>();

        private bool _dirty = true;
        
        public override void Render(bool forceUpdate)
        {
            if (!forceUpdate && !_dirty)
                return;

            _dirty = false;

            Clear();

            var y = Size.Top;
            var firstLine = Math.Max(0, _lines.Count - Size.Height);
            for (var i = firstLine; i < Math.Min(firstLine + Size.Height, _lines.Count); i++)
            {
                Print(0, y, _lines[i]);
                y++;
            }
        }

        protected override void OnResized()
        {
            // If the width has changed, the lines need to be recalculated.
            _lines.Clear();
            foreach (var msg in _messages)
                WordWrap(msg);

            _dirty = true;
        }

        void Print(int x, int y, string msg)
        {
            for(int i = 0; i < Math.Min(msg.Length, Size.Width); i++)
            {
                CellGrid.Cells[x, y].Ascii = msg[i];
                x++;
            }
        }

        private void WordWrap(string msg)
        {
            var startPos = 0;
            var endPos = 0;

            var maxWidth = Size.Width - 2;

            for (var i = 0; i < msg.Length; i++)
            {
                if (msg[i] == ' ')
                {
                    if (i - startPos > maxWidth)
                    {
                        _lines.Add((startPos == 0 ? "* " : "  ") + msg.Substring(startPos, endPos - startPos));

                        startPos = endPos + 1;
                    }
                    endPos = i;
                }
            }
            _lines.Add((startPos == 0 ? "* " : "  ") + msg.Substring(startPos, msg.Length - startPos));
        }

        public void AddMessage(string msg)
        {
            _messages.Add(msg);
            WordWrap(msg);
            _dirty = true;
        }
    }
}
