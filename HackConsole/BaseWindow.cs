﻿using System;

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

        protected bool _dirty;
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
            var popup = PopupStack.WidgetAt(pos);

            if (popup != null)
                return popup;

            return WidgetAt(pos, Widgets);
        }

        private static Widget WidgetAt(Vec pos, WidgetContainer container)
        {
            while (true)
            {
                Widget top = null;
                foreach (var w in container.Widgets)
                    if (w.Size.Contains(pos))
                        top = w;

                container = top as WidgetContainer;
                if (container == null)
                    return top;
            }
        }

        public abstract void Run();
    }

    public static class WindowData
    {
        public static Grid<Symbol> Data;
    }
}