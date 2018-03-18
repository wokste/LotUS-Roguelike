using System.Runtime.InteropServices;

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
    }
}
