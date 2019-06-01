using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace HackConsole
{
    public abstract class Widget 
    {
        public Rect Rect;
        public Rect DesiredSize;
        public Docking Docking = Docking.None;
        public View View = new View();

        public Widget Parent;
        public Widget Ancestor => Parent == null ? this : Parent.Ancestor;

        public void Draw(RenderTarget target)
        {
            target.SetView(View);
            DrawInternal(target);
        }

        protected abstract void DrawInternal(RenderTarget target);

        //public abstract bool CanHasFocus { get; }

        /// <summary>
        /// This will automatically be called when the widget is resized.
        /// </summary>
        protected virtual void OnResized() {}

        public void ResizeDocked(ref Rect free)
        {
            var newSize = MakeDockingSize(ref free);

            Resize(newSize);
        }

        public void Resize(Rect newSize)
        {
            if (Rect.Left == newSize.Left && Rect.Width == newSize.Width && Rect.Top == newSize.Top && Rect.Height == newSize.Height)
                return;

            Rect = newSize;
            ResizeView();
            OnResized();
        }

        private void ResizeView()
        {
            View.Size = new Vector2f(Rect.Width, Rect.Height);
            View.Center = new Vector2f(Rect.Width / 2, Rect.Height / 2);

            var screenRect = Ancestor.Rect;

            Debug.Assert(screenRect.Left == 0);
            Debug.Assert(screenRect.Top == 0);

            var rect = new FloatRect((float)Rect.Left / screenRect.Width, (float)Rect.Top / screenRect.Height, (float)Rect.Width / screenRect.Width, (float)Rect.Height / screenRect.Height);
            View.Viewport = rect;
        }

        public void CenterPopup(Rect screen)
        {
            var newSize = DesiredSize;
            newSize.Left = screen.Left + (screen.Width - newSize.Width) / 2;
            newSize.Top = screen.Top + (screen.Height - newSize.Height) / 2;

            Resize(newSize);
        }

        private Rect MakeDockingSize(ref Rect free)
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
                default:
                    Debug.Assert(false);
                    return new Rect(free.TopLeft, DesiredSize.Size);
            }
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
