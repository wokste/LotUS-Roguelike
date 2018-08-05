using System;

namespace HackConsole
{
    public abstract class BaseWindow
    {
        protected uint _windowWidth = 1280;
        protected uint _windowHeight = 800;

        protected readonly uint _fontX = 16;
        protected readonly uint _fontY = 16;

        public WidgetContainer Widgets = new WidgetContainer { Docking = Docking.Fill };

        public IKeyEventSuscriber BaseKeyHandler;
        public Action OnUpdate;
        public PopupStack PopupStack = new PopupStack();

        protected Vec? _lastMousePos;

        protected void ResizeScreen(uint x, uint y)
        {
            _windowWidth = x;
            _windowHeight = y;
            var size = new Size((int)(_windowWidth / _fontX), (int)(_windowHeight / _fontY));

            var r = new Rect(Vec.Zero, size);
            PopupStack.Resize(ref r);

            r = new Rect(Vec.Zero, size);
            Widgets.Resize(ref r);
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