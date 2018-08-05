using System;
using SFML.Graphics;

namespace HackConsole
{
    public abstract class Widget : SFML.Graphics.Drawable
    {
        public Rect Rect;
        public Rect DesiredSize;
        public Docking Docking = Docking.None;
        public bool Dirty = true;

        public abstract void Draw(RenderTarget target, RenderStates states);

        //public abstract bool CanHasFocus { get; }

        /// <summary>
        /// This will automatically be called when the widget is resized.
        /// </summary>
        protected virtual void OnResized() {}

        public void Resize(ref Rect free)
        {
            var newSize = MakeSize(ref free);

            if (Rect.Left == newSize.Left && Rect.Width == newSize.Width && Rect.Top == newSize.Top && Rect.Height == newSize.Height)
                return;

            Rect = newSize;

            OnResized();
            Dirty = true;
        }

        public void CenterPopup(Rect screen)
        {
            var newSize = DesiredSize;
            newSize.Left = screen.Left + (screen.Width - newSize.Width) / 2;
            newSize.Top = screen.Top + (screen.Height - newSize.Height) / 2;

            if (Rect.Left == newSize.Left && Rect.Width == newSize.Width && Rect.Top == newSize.Top && Rect.Height == newSize.Height)
                return;

            Rect = newSize;

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

            return new Rect(free.TopLeft, DesiredSize.Size);
        }

        public virtual Widget WidgetAt(Vec pos)
        {
            return this;
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
