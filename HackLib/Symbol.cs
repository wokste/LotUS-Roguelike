namespace HackLib
{
    public struct Symbol
    {
        public char Ascii;
        public int TextColor;
        public int BackgroundColor;

        public Symbol(char ascii, int textColor = Color.White, int backgroundColor = Color.Black)
        {
            Ascii = ascii;
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
    }

    internal struct Color
    {
        internal const int White  = 0xffffff;
        internal const int Black  = 0x000000;
        internal const int Red    = 0xff0000;
        internal const int Green  = 0x00ff00;
        internal const int Blue   = 0x0000ff;
        internal const int Yellow = 0xffff00;
        internal const int Orange = 0xff8000;
        internal const int Cyan   = 0x00ffff;
        internal const int Pink   = 0xff00ff;
        internal const int Gray   = 0x808080;
    }
}
