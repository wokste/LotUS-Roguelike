using System;

namespace HackConsole
{
    public abstract class Widget
    {
        public Rect Size;
        public Rect DesiredSize;
        public Docking Docking = Docking.None;
        
        /// <summary>
        /// Renders the widget on the CellGrid.
        /// </summary>
        /// <param name="forceUpdate">If this is false, the widget may assume that the previous content is still on the same location.</param>
        public abstract void Render(bool forceUpdate);

        /// <summary>
        /// This will automatically be called when the widget is resized.
        /// </summary>
        protected virtual void OnResized()
        {
        }

        /// <summary>
        /// helper function to clear the area of the widget.
        /// </summary>
        protected void Clear()
        {
            for (var y = Size.Top; y < Size.Bottom; y++)
            {
                for (var x = Size.Left; x < Size.Right; x++)
                {
                    CellGrid.Cells[x, y] = new Symbol {Ascii = ' ', BackgroundColor = Color.Black, TextColor = Color.Yellow};
                }
            }
        }

        protected void Print(int x, int y, string msg, Color fgColor, Color bgColor = default(Color))
        {
            x += Size.Left;
            var length = Math.Min(msg.Length, Size.Right - x);
            y += Size.Top;

            for (int i = 0; i < length; i++)
            {
                CellGrid.Cells[x + i, y] = new Symbol {Ascii = msg[i], BackgroundColor = bgColor, TextColor = fgColor};
            }
        }

        public void Resize(ref Rect free)
        {
            var newSize = MakeSize(ref free);

            if (Size.Left == newSize.Left && Size.Width == newSize.Width && Size.Top == newSize.Top && Size.Height == newSize.Height)
                return;

            Size = newSize;

            OnResized();
        }

        private Rect MakeSize(ref Rect free)
        {
            if (Docking == Docking.Top || Docking == Docking.Bottom)
            {
                var newSize = new Rect
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
                var newSize = new Rect
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
