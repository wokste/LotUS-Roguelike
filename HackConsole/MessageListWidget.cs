using SFML.Graphics;
using System;
using System.Collections.Generic;
using HackConsole.Ui;

namespace HackConsole
{
    public class MessageListWidget : Widget //, IMouseEventSuscriber
    {
        private readonly List<ColoredString> _messages = new List<ColoredString>();
        protected readonly List<string> Lines = new List<string>();
        
        private readonly VertexArray _vertices = new VertexArray();
        public BitmapFont Font;
        private bool _dirty = true;

        private int _scrollY;
        protected int ScrollY {
            get => _scrollY;
            set {
                var max = Math.Max(0, Lines.Count - Rect.Height);
                _scrollY = MyMath.Clamp(value, 0, max);
            }
        }

        private int _bottomY;

        public MessageListWidget(BitmapFont font = null)
        {
            Font = font ?? Sprites.Font;
            _vertices.PrimitiveType = PrimitiveType.Quads;
        }


        public override void Draw(RenderTarget target)
        {
            if (_dirty == true)
            {
                Render();
                _dirty = false;
            }

            var states = new RenderStates(Font.Texture);

            target.Draw(_vertices,states);
        }

        protected void Render()
        {
            _vertices.Clear();
            _bottomY = 0;

            foreach (var l in _messages)
            {
                RenderLine(l.Text);
            }

            /*
            var firstLine = _posY;
            var i = firstLine;
            while (i < Lines.Count && y < Data.Height)
            {
                Print(new Vec(0, y), Lines[i], Colour.White);
                i++;
                y++;
            }
            */
        }


        protected override void OnResized()
        {
            base.OnResized();
            _dirty = true;
        }

        private void RenderLine(string msg)
        {
            // TODO: Stuff
            Font.Print(_vertices, msg, Rect.Width, new Vec(0, Rect.Top + _bottomY));
            ScrollY += Font.LineHeight + Font.SpacingV;
            _bottomY += Font.LineHeight + Font.SpacingV;
        }

        public void Add(ColoredString msg)
        {
            _messages.Add(msg);
            RenderLine(msg.Text);
            /*var range = StringExt.Prefix(StringExt.Wrap(msg.Text, Rect.Width - 2), "> ");
            PosY += range.Count();
            Lines.AddRange(range);
            Dirty = true;*/
        }

        public virtual void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
        }

        public virtual void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
            ScrollY -= delta.Y * 10;
        }
    }
}
