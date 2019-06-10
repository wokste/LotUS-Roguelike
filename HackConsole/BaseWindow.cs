using System;

namespace HackConsole
{
    public abstract class BaseWindow
    {
        protected uint _windowWidth = 1280;
        protected uint _windowHeight = 800;

        public WidgetContainer Widgets = new WidgetContainer { Docking = Docking.Fill };

        public IKeyEventSuscriber BaseKeyHandler;
        public Action OnUpdate;
        public PopupStack PopupStack = new PopupStack();

        protected Vec? _lastMousePos;

        protected void ResizeScreen(uint x, uint y)
        {
            var size = new Size((int)x, (int)y);

            var r = new Rect(Vec.Zero, size);
            PopupStack.ResizeDocked(ref r);

            r = new Rect(Vec.Zero, size);
            Widgets.ResizeDocked(ref r);
        }

        protected Widget WidgetAt(Vec pos)
        {
            if (!PopupStack.Empty)
            {
                var topStackWidget = PopupStack.Top;
                if (topStackWidget.Rect.Contains(pos))
                    return topStackWidget;
                else
                    return null;
            }
            return Widgets.WidgetAt(pos);
        }

        public abstract void Run();
    }
}