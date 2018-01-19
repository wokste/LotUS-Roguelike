using System;

namespace HackConsole
{
    public abstract class Widget
    {
        public CRect Size;
        public CRect DesiredSize;
        public Docking Docking = Docking.None;

        public abstract void Render();

        protected virtual void OnResized()
        {
        }

        protected void Clear()
        {
            for (var y = Size.Top; y < Size.Bottom; y++)
            {
                for (var x = Size.Left; x < Size.Right; x++)
                {
                    CellGrid.Cells[x, y] = new Cell {Ascii = ' '};
                }
            }
        }

        public void Resize(ref CRect free)
        {
            var newSize = MakeSize(ref free);

            if (Size.Left == newSize.Left && Size.Width == newSize.Width && Size.Top == newSize.Top && Size.Height == newSize.Height)
                return;

            Size = newSize;

            OnResized();
        }

        private CRect MakeSize(ref CRect free)
        {
            if (Docking == Docking.Top || Docking == Docking.Bottom)
            {
                var newSize = new CRect
                {
                    Left = free.Left,
                    Width = free.Width,
                    Top = Docking == Docking.Top ? free.Top : free.Bottom - DesiredSize.Height,
                    Height = DesiredSize.Height
                };
                
                free.Height -= newSize.Height;
                if (Docking == Docking.Top)
                    free.Top += newSize.Height;

                return newSize;
            }
            if (Docking == Docking.Left || Docking == Docking.Right)
            {
                var newSize = new CRect
                {
                    Left = Docking == Docking.Left ? free.Left : free.Right - DesiredSize.Width,
                    Width = DesiredSize.Width,
                    Top = free.Top,
                    Height = free.Height
                };

                free.Width -= newSize.Width;
                if (Docking == Docking.Top)
                    free.Left += newSize.Width;

                return newSize;
            }
            if (Docking == Docking.Fill)
                return free;

            return DesiredSize;
        }
    }

    public struct CRect
    {
        public int Left;
        public int Top;
        public int Width;
        public int Height;

        public int Right => Left + Width;
        public int Bottom => Top + Height;

        public bool Contains(int x, int y)
        {
            return (x >= Left) && (y >= Top) && (x < Right) && (y < Bottom);
        }
    }

    public enum Docking
    {
        None,
        Top,
        Right,
        Bottom,
        Left,
        Fill
    }
}
