using System;
using System.Collections.Generic;

namespace HackConsole
{
    public struct ColoredString
    {
        public string Text;
        public Colour Color;
        public int Length => Text.Length;

        private ColoredString(string text, Colour color)
        {
            Text = text;
            Color = color;
        }

        public static void Write(string text, Colour color)
        {
            OnMessage?.Invoke(new ColoredString(text, color));
        }

        public Symbol this[int index] {
            get {
                return new Symbol(Text[index], Color, Colour.Transparent);
            }
        }

        public static Action<ColoredString> OnMessage;

        internal IEnumerable<(char, Colour)> Iterate()
        {
            foreach (var c in Text)
            {
                yield return (c, Color);
            }
        }
    }
}
