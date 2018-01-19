namespace HackConsole
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

    public struct Color
    {
        public const int White  = 0xffffff;
        public const int Black  = 0x000000;
        public const int Red    = 0xff0000;
        public const int Green  = 0x00ff00;
        public const int Blue   = 0x0000ff;
        public const int Yellow = 0xffff00;
        public const int Orange = 0xff8000;
        public const int Cyan   = 0x00ffff;
        public const int Pink   = 0xff00ff;
        public const int Gray   = 0x808080;
    }
}
