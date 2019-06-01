using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HackConsole.Ui
{
    public class BitmapFont
    {
        private struct BfChar {
            internal int Left;
            internal int Width;
            internal int Top;

            public BfChar(int left, int width, int top) : this()
            {
                Left = left;
                Width = width;
                Top = top;
            }
        };

        public Texture Texture;
        public Size Size;
        public readonly int LineHeight;
        public readonly int SpacingH;
        public readonly int SpacingV;

        private readonly Dictionary<char, BfChar> _charData = new Dictionary<char, BfChar>();

        public BitmapFont(string name, int lineHeight, int spacingH, int spacingV)
        {
            LineHeight = lineHeight;
            SpacingH = spacingH;
            SpacingV = spacingV;

            var image = new Image($"{name}.png");
            Texture = new Texture(image);

            using (var reader = System.IO.File.OpenText($"{name}.csv"))
            {
                var header = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');
                    Debug.Assert(values.Length == 4);

                    var c = (values[0] == "\\0" ? '\0' :  values[0][0]);

                    _charData[values[0][0]] = new BfChar(Int32.Parse(values[1]), Int32.Parse(values[2]), Int32.Parse(values[3]));
                }
            }

            Debug.WriteLine(_charData.Count);
        }

        public void Print(VertexArray vertexArray, ColoredString text, int width, Vec v)
        {
            Vector2f posF = new Vector2f(v.X, v.Y);
            foreach ((var c, var color) in text.Iterate())
            {

                if (!_charData.TryGetValue(c, out BfChar bitmapChar) && !_charData.TryGetValue('\0', out bitmapChar))
                    throw new Exception();

                PrintChar(vertexArray, bitmapChar, posF, color.ToSfmlColor());
                posF.X += bitmapChar.Width + SpacingH;
            }
        }

        private void PrintChar(VertexArray vertexArray, BfChar c, Vector2f posF, SFML.Graphics.Color color)
        {
            var white = new SFML.Graphics.Color(255, 255, 255, 255);
            var texCoord = new Vector2f(c.Left, c.Top);
            var vX = new Vector2f(c.Width, 0);
            var vY = new Vector2f(0, LineHeight); // TODO: 12 is a magic number indicating font height.
            var vXY = vX + vY;

            vertexArray.Append(new Vertex(posF, white, texCoord));
            vertexArray.Append(new Vertex(posF + vX, white, texCoord + vX));
            vertexArray.Append(new Vertex(posF + vXY, color, texCoord + vXY));
            vertexArray.Append(new Vertex(posF + vY, color, texCoord + vY));
        }
    }
}
