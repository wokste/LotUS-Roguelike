using System;

namespace HackConsole
{
    public abstract class Widget
    {
        public Rect Size;
        public Rect DesiredSize;
        public Docking Docking = Docking.None;

        //public abstract bool CanHasFocus { get; }

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

        #region HelperFunctions

        /// <summary>
        /// Clear the area of the widget.
        /// </summary>
        protected void Clear()
        {
            foreach (var v in Size.Iterator())
            {
                WindowData.Data[v] = new Symbol { Ascii = ' ', BackgroundColor = Color.Black, TextColor = Color.Yellow };
            }
        }

        /// <summary>
        /// Print a message at the (X,Y) position, relative to the TopLeft of the widget
        /// </summary>
        /// <param name="v">Relative position to topleft of widget</param>
        /// <param name="msg">The text.</param>
        /// <param name="fgColor">Foreground color</param>
        /// <param name="bgColor">Background color</param>
        protected void Print(Vec v, string msg, Color fgColor, Color bgColor = default(Color))
        {
            v += Size.TopLeft;
            var length = Math.Min(msg.Length, Size.Right - v.X);

            for (var i = 0; i < length; i++)
            {
                WindowData.Data[new Vec(v.X + i, v.Y)] = new Symbol { Ascii = msg[i], BackgroundColor = bgColor, TextColor = fgColor };
            }
        }

        #endregion


        public void Resize(ref Rect free)
        {
            var newSize = MakeSize(ref free);

            if (Size.Left == newSize.Left && Size.Width == newSize.Width && Size.Top == newSize.Top && Size.Height == newSize.Height)
                return;

            Size = newSize;

            OnResized();
        }

        public void CenterPopup(Rect screen)
        {
            var newSize = DesiredSize;
            newSize.Left = screen.Left + (screen.Width - newSize.Width) / 2;
            newSize.Top = screen.Top + (screen.Height - newSize.Height) / 2;

            if (Size.Left == newSize.Left && Size.Width == newSize.Width && Size.Top == newSize.Top && Size.Height == newSize.Height)
                return;

            Size = newSize;

            OnResized();
        }

        private Rect MakeSize(ref Rect free)
        {
            switch (Docking)
            {
                case Docking.Top:
                case Docking.Bottom:
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
                case Docking.Left:
                case Docking.Right:
                {
                    var newSize = new Rect
                    {
                        Left = (Docking == Docking.Left) ? free.Left : free.Right - DesiredSize.Width,
                        Width = DesiredSize.Width,
                        Top = free.Top,
                        Height = free.Height
                    };

                    free.Width -= newSize.Width;
                    if (Docking == Docking.Left)
                        free.Left += newSize.Width;

                    return newSize;
                }
                case Docking.Fill:
                    return free;
            }

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
