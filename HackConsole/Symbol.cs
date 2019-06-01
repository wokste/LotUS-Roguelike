using System.Runtime.InteropServices;
using System.Linq;
using System;
using SFML.Graphics;

namespace HackConsole
{
    public struct Symbol
    {
        public char Ascii;
        public Color TextColor;
        public Color BackgroundColor;

        public Symbol(char ascii, Color textColor, Color backgroundColor = default(Color))
        {
            Ascii = ascii;
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
    }

    public static class ColorExtentions
    {
        public static Color? TryParse(string text) {
            if (text == null)
                return null;

            if (text.StartsWith("#"))
            {
                var hex = text.Substring(1);

                if (hex.Length <= 4)
                    hex = string.Join("", hex.ToArray().Select(c => $"{c}{c}"));

                if (hex.Length != 6 && hex.Length != 8)
                    return null;

                if (!int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var argb))
                    return null;

                return new Color((byte)((argb & 0xff0000) >> 16), (byte)((argb & 0xff00) >> 8), (byte)(argb & 0xff), (byte)((argb & -16777216) >> 24));
            }
            else
            {
                return null;
            }
        }

        public static Color Parse(string text)
        {
            var c = TryParse(text);
            if (c is Color cc)
                return cc;

            throw new FormatException($"{text} is not a valid color");
        }
    }
}
