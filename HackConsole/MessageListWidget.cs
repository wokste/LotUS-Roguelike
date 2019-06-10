using SFML.Graphics;
using System;
using System.Collections.Generic;
using HackConsole.Ui;

namespace HackConsole
{
    public class MessageListWidget : Widget, IMouseEventSuscriber
    {
        private readonly List<string> _messages = new List<string>();
        protected readonly List<string> Lines = new List<string>();
        
        private readonly VertexArray _vertices = new VertexArray();
        public BitmapFont Font;
        private bool _dirty = true;
        
        private int _scrollY;
        protected int ScrollY {
            get => _scrollY;
            set {
                var max = Math.Max(0, _bottomY - Rect.Height);
                _scrollY = MyMath.Clamp(value, 0, max);
            }
        }

        private int _bottomY;

        public MessageListWidget(BitmapFont font = null)
        {
            Font = font ?? Sprites.Font;
            _vertices.PrimitiveType = PrimitiveType.Quads;
        }


        protected override void DrawInternal(RenderTarget target)
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
                RenderLine(l);
            }

            UpdateView(true);
        }

        protected override void OnResized()
        {
            base.OnResized();
            //_dirty = true;
        }

        private void RenderLine(string msg)
        {
            // TODO: Stuff
            Font.Print(_vertices, msg, Rect.Width, new Vec(0, _bottomY));

            _bottomY += Font.LineHeight + Font.SpacingV;
        }

        public void Add(string msg)
        {
            _messages.Add(msg);
            RenderLine(msg);
            
            UpdateView(true);
        }

        public virtual void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
        }

        public virtual void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
            ScrollY += delta.Y * -10;
            UpdateView(false);
        }

        private void UpdateView(bool scrollToBottom = true) {
            if (scrollToBottom)
                ScrollY = int.MaxValue; // Will be clamped

            var center = Rect.Size.Center;
            View.Center = new SFML.Window.Vector2f(center.X, center.Y + ScrollY);
        }
    }
}
