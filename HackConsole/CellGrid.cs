namespace HackConsole
{
    public static class CellGrid
    {
        public static uint Width { get; private set; }
        public static uint Height { get; private set; }
        public static Symbol[,] Cells { get; private set; }

        public static void Resize(uint width, uint height)
        {
            Width = width;
            Height = height;

            Cells = new Symbol[width, height];
        }
    }
}
