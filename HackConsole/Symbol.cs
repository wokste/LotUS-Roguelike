using System.Runtime.InteropServices;
using System.Linq;
using System;

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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color
    {
        public static readonly Color White = new Color(255, 255, 255);
        public static readonly Color Black = new Color(0, 0, 0);
        public static readonly Color Red = new Color(255, 0, 0);
        public static readonly Color Green = new Color(0, 255, 0);
        public static readonly Color Blue = new Color(0, 0, 255);
        public static readonly Color Yellow = new Color(255, 255, 0);
        public static readonly Color Orange = new Color(255, 128, 0);
        public static readonly Color Cyan = new Color(0, 255, 255);
        public static readonly Color Pink = new Color(255, 0, 255);
        public static readonly Color Gray = new Color(128, 128, 128);
        public static readonly Color Transparent = new Color(0, 0, 0, 0);

        public byte R;
        public byte G;
        public byte B;
        public byte A;
        
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public void Darken(byte mult)
        {
            R = (byte)(R * mult / 255);
            G = (byte)(G * mult / 255);
            B = (byte)(B * mult / 255);
        }

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
