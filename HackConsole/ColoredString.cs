using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole
{
    public struct ColoredString
    {
        public string Text;
        public Color Color;
        public int Length => Text.Length;

        private ColoredString(string text, Color color)
        {
            Text = text;
            Color = color;
        }

        public static void Write(string text, Color color)
        {
            OnMessage?.Invoke(new ColoredString(text, color));
        }

        public Symbol this[int index] {
            get {
                return new Symbol(Text[index], Color, Color.Transparent);
            }
        }

        public static Action<ColoredString> OnMessage;
    }
}
