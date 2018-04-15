namespace HackConsole
{
    public static class CellGrid
    {
        public static Vec Size { get; private set; }
        public static Symbol[,] Cells { get; private set; }

        public static void Resize(Vec size)
        {
            Size = size;

            Cells = new Symbol[Size.X, Size.Y];
        }
    }
}
