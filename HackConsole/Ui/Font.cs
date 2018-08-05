using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole.Ui
{
    public class Font
    {
        public Texture Texture;
        public Size Size;

        public Font(string texName)
        {
            var image = new Image($"{texName}");
            Texture = new Texture(image);
        }

        public static Font Mono;

        /*
        /// <summary>
        /// Print a message at the (X,Y) position, relative to the TopLeft of the widget
        /// </summary>
        /// <param name="v">Relative position to topleft of widget</param>
        /// <param name="msg">The text.</param>
        /// <param name="color">Foreground color</param>
        protected bool Print(Rect area, ColoredString[] msg, bool wordWrap)
        {
            var width = 

            for (var i = 0; i < length; i++)
            {
                WindowData.Data[new Vec(v.X + i, v.Y)] = new Symbol { Ascii = msg[i], BackgroundColor = bgColor, TextColor = color };
            }
            return true;

        }

        private IEnumerable<string> Wrap(string msg, int maxWidth, ref Vec cursor)// string prefix)
        {
            int FirstChar(int pos)
            {
                for (int i = pos; i < msg.Length; i++)
                    if (!(msg[i] == ' ' || msg[i] == '\n'))
                        return i;
                return msg.Length;
            }

            var lineStart = 0;
            var lastSpace = 0;

            for (var i = 0; i < msg.Length; i++)
            {
                var c = msg[i];

                switch (msg[i])
                {
                    case ' ':
                        lastSpace = i;
                        break;
                    case '\n':
                        {
                            var lineEnd = i;
                            yield return msg.Substring(lineStart, Math.Max(0, lineEnd - lineStart));
                            lineStart = FirstChar(lineEnd);
                        }
                        break;
                    default:
                        if (i - lineStart >= maxWidth)
                        {
                            var lineEnd = (lastSpace > lineStart) ? lastSpace : i;
                            yield return msg.Substring(lineStart, Math.Max(0, lineEnd - lineStart));
                            lineStart = FirstChar(lineEnd);
                        }
                        break;
                }
            }

            if (msg.Length != lineStart)
            {
                yield return msg.Substring(lineStart, msg.Length - lineStart);
            }
        }

        public static string CleanUp(this string source)
        {
            var sb = new StringBuilder(source.Length);

            bool firstChar = true;
            bool space = false;

            foreach (var c in source)
            {
                // Max 1 space to be added
                if (c == ' ')
                {
                    space = true;
                    continue;
                }
                if (space)
                {
                    space = false;
                    sb.Append(' ');
                }

                // ToUpper all first characters
                sb.Append(firstChar ? char.ToUpper(c) : c);
                if (c == '\t' || c == ' ' || c == '\n' || c == '\r')
                    continue;

                firstChar = (c == '.' || c == '?' || c == '!');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Print a message at the (X,Y) position, relative to the TopLeft of the widget
        /// </summary>
        /// <param name="v">Relative position to topleft of widget</param>
        /// <param name="msg">The text.</param>
        protected bool Print(Vec v, ColoredString msg)
        {
            
        }

        protected void Print(Vec v, Symbol s)
        {
            v += Size.TopLeft;
            WindowData.Data[v] = s;
        }

        #endregion
        */
    }
}
