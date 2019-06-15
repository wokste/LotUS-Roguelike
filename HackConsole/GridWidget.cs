using System;
using SFML.Graphics;
using SFML.Window;

namespace HackConsole
{
    public abstract class GridWidget : Widget
    {
        private readonly VertexArray _vertices = new VertexArray();
        public Grid<Symbol> Data;
        protected readonly int _fontX = 16;
        protected readonly int _fontY = 16;

        public bool Dirty = true;

        protected override void OnResized()
        {
            base.OnResized();
            ResizeVertices();
            Dirty = true;
        }

        // Inspired by https://github.com/thebracket/rltk/blob/master/rltk/virtual_terminal.cpp
        protected override void DrawInternal(RenderTarget target)
        {
            if (Dirty)
            {
                Render();
                var spaceAscii = 219;
                var spacePos = new Vector2f((spaceAscii % 16) * _fontX, (spaceAscii / 16) * _fontY);

                var d = new Vector2f[] {
                    new Vector2f(0, 0),
                    new Vector2f(_fontX, 0),
                    new Vector2f(_fontX, _fontY),
                    new Vector2f(0, _fontY),
                };

                foreach (var v in Data.Ids())
                {
                    var idx = (uint)((v.Y * Data.Size.X) + v.X) * 8;
                    var vecScreen = new Vector2f((v.X * _fontX), (v.Y * _fontY));

                    var Char = Data[v];
                    var texPos = new Vector2f((Char.Ascii % 16) * _fontX, (Char.Ascii / 16) * _fontY);

                    var bgColor = Char.BackgroundColor;
                    var fgColor = Char.TextColor;

                    for (uint i = 0; i < 4; ++i)
                    {
                        _vertices[idx + i] = new Vertex(vecScreen + d[i], bgColor, spacePos + d[i]);
                        _vertices[idx + i + 4] = new Vertex(vecScreen + d[i], fgColor, texPos + d[i]);
                    }
                }
                Dirty = false;
            }
            var states = new RenderStates(Ui.Sprites.Ascii);
            target.Draw(_vertices, states);
        }

        protected abstract void Render();

        void ResizeVertices()
        {
            var newSize = new Size(Rect.Width / 16, Rect.Height / 16);

            if (Data != null && newSize == Data.Size)
                return;

            Data = new Grid<Symbol>(newSize);

            // Build the vertex buffer
            _vertices.PrimitiveType = PrimitiveType.Quads;
            _vertices.Resize((uint)newSize.Area * 8);
        }

        #region HelperFunctions

        /// <summary>
        /// Clear the area of the widget.
        /// </summary>
        protected void Clear(Color bg)
        {
            foreach (var v in Data.Ids())
            {
                Data[v] = new Symbol { Ascii = ' ', BackgroundColor = Color.Black, TextColor = Color.White };
            }
        }

        /// <summary>
        /// Print a message at the (X,Y) position, relative to the TopLeft of the widget
        /// </summary>
        /// <param name="v">Relative position to topleft of widget</param>
        /// <param name="msg">The text.</param>
        /// <param name="fgColor">Foreground color</param>
        /// <param name="bgColor">Background color</param>
        protected bool Print(Vec v, string msg, Color fgColor, Color bgColor = default(Color))
        {
            if (v.Y >= Data.Height)
                return false;

            var length = Math.Min(msg.Length, Data.Size.X - v.X);

            for (var i = 0; i < length; i++)
            {
                var bg = bgColor.A == 0 ? Data[new Vec(v.X + i, v.Y)].BackgroundColor : bgColor;
                Data[new Vec(v.X + i, v.Y)] = new Symbol { Ascii = msg[i], BackgroundColor = bg, TextColor = fgColor };
            }
            return true;
        }

        protected void Print(Vec v, Symbol s)
        {
            if (v.Y >= Data.Height)
                return;

            Data[v] = s;
        }

        #endregion
    }
}
