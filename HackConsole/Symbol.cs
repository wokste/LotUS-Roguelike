using System.Runtime.InteropServices;
using System.Linq;
using System;
using SFML.Graphics;

namespace HackConsole
{
    public struct Symbol
    {
        public char Ascii;
        public Colour TextColor;
        public Colour BackgroundColor;

        public Symbol(char ascii, Colour textColor, Colour backgroundColor = default(Colour))
        {
            Ascii = ascii;
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }

        public Symbol Darken(byte mult)
        {
            if (mult == 255)
                return this;

            return new Symbol(Ascii, TextColor.Darken(mult), BackgroundColor.Darken(mult));
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Colour
    {
        public static readonly Colour White = new Colour(255, 255, 255);
        public static readonly Colour Black = new Colour(0, 0, 0);
        public static readonly Colour Red = new Colour(255, 0, 0);
        public static readonly Colour Green = new Colour(0, 255, 0);
        public static readonly Colour Blue = new Colour(0, 0, 255);
        public static readonly Colour Yellow = new Colour(255, 255, 0);
        public static readonly Colour Orange = new Colour(255, 128, 0);
        public static readonly Colour Cyan = new Colour(0, 255, 255);
        public static readonly Colour Pink = new Colour(255, 0, 255);
        public static readonly Colour Gray = new Colour(128, 128, 128);
        public static readonly Colour Transparent = new Colour(0, 0, 0, 0);

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Colour(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public Colour Darken(byte mult)
        {
            return new Colour(
                (byte)(R * mult / 255),
                (byte)(G * mult / 255),
                (byte)(B * mult / 255),
                A);
        }

        public void Add(Colour other)
        {
            var f = other.A;
            R = (byte)((R * (255 - f) + other.R * f) / 256);
            G = (byte)((G * (255 - f) + other.G * f) / 256);
            B = (byte)((B * (255 - f) + other.B * f) / 256);
            A = 255;
        }

        internal Color ToSfmlColor()
        {
            return new Color(R, G, B, A);
        }

        public static Colour? TryParse(string text) {
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

                return new Colour((byte)((argb & 0xff0000) >> 16), (byte)((argb & 0xff00) >> 8), (byte)(argb & 0xff), (byte)((argb & -16777216) >> 24));
            }
            else
            {
                return null;
            }
        }

        public static Colour Parse(string text)
        {
            var c = TryParse(text);
            if (c is Colour cc)
                return cc;

            throw new FormatException($"{text} is not a valid color");
        }
    }
}
