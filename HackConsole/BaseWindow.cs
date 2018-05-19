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
            WindowData.Data = new Grid<Symbol>(new Vec((int)(_windowWidth / _fontX), (int)(_windowHeight / _fontY)));

            var r = new Rect(Vec.Zero, WindowData.Data.Size);
            PopupStack.Resize(ref r);

            r = new Rect(Vec.Zero, WindowData.Data.Size);
            Widgets.Resize(ref r);
        }

        protected Widget WidgetAt(Vec pos)
        {
            return PopupStack.WidgetAt(pos) ?? Widgets.WidgetAt(pos);
        }

        protected bool RenderWidgets()
        {
            var dirty = WindowData.ForceUpdate;
            WindowData.ForceUpdate = false;

            var rendered = Widgets.Render(dirty);
            rendered |= PopupStack.Render(dirty || rendered);

            return rendered;
        }

        public abstract void Run();
    }

    public static class WindowData
    {
        public static Grid<Symbol> Data;
        public static bool ForceUpdate = true;
    }
}