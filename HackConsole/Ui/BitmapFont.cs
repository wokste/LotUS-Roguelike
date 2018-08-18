using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Dictionary<char, BfChar> _charData = new Dictionary<char, BfChar>();

        public BitmapFont(string name)
        {
            var image = new Image($"{name}.png");
            Texture = new Texture(image);

            using (var reader = System.IO.File.OpenText($"{name}.csv"))
            {
                var header = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    Debug.Assert(values.Length == 4);

                    var c = values[0] == "%COMMA%" ? ',' :  (values[0] == "%WTF%" ? '\0' :  values[0][0]);

                    _charData[values[0][0]] = new BfChar(Int32.Parse(values[1]), Int32.Parse(values[2]), Int32.Parse(values[3]));
                }
            }

            Debug.WriteLine(_charData.Count);
        }

        public void Print(VertexArray vertexArray, string text, Rect r)
        {
            Vector2f posF = new Vector2f(r.Left, r.Top);
            foreach (var c in text)
            {
                BfChar bitmapChar;

                if (!_charData.TryGetValue(c, out bitmapChar) && !_charData.TryGetValue('\0', out bitmapChar))
                    throw new Exception();

                PrintChar(vertexArray, bitmapChar, posF);
                posF.X += bitmapChar.Width;
            }
        }

        private void PrintChar(VertexArray vertexArray, BfChar c, Vector2f posF)
        {
            var topColor = new SFML.Graphics.Color(255, 255, 255, 255);
            var bottomColor = new SFML.Graphics.Color(255, 128, 128, 255);
            var texCoord = new Vector2f(c.Left, c.Top);
            var vX = new Vector2f(c.Width, 0);
            var vY = new Vector2f(0, 12); // TODO: 12 is a magic number indicating font height.

            vertexArray.Append(new Vertex(posF, topColor, texCoord));
            vertexArray.Append(new Vertex(posF + vX, topColor, texCoord + vX));
            vertexArray.Append(new Vertex(posF + vX + vY, bottomColor, texCoord + vX + vY));
            vertexArray.Append(new Vertex(posF + vY, bottomColor, texCoord + vY));
        }
    }
}
