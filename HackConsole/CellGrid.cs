using System;

namespace HackConsole
{
    public static class CellGrid
    {
        public static uint Width { get; private set; }
        public static uint Height { get; private set; }
        public static Cell[,] Cells { get; private set; }

        public static void Resize(uint width, uint height)
        {
            Width = width;
            Height = height;

            Cells = new Cell[width, height];

            //TODO: Remove temporary code
            var rnd = new Random();
            for(var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    Cells[x,y].Ascii = (char) rnd.Next((int) 'a', (int) 'z');
        }
    }

    public struct Cell
    {
        public char Ascii;
    }
}
