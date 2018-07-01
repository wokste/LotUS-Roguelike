using System;

namespace HackConsole
{
    public class BorderedWidget : Widget, IPopupWidget, IKeyEventSuscriber, IMouseEventSuscriber
    {
        Widget InnerWidget;
        Color FcColor = Color.White;
        Color BgColor = Color.Gray;

        public BorderedWidget(Widget inner) {
            InnerWidget = inner;
            DesiredSize = InnerWidget.DesiredSize.Grow(1);
        }

        public Action OnClose {
            get => (InnerWidget as IPopupWidget).OnClose;
            set => (InnerWidget as IPopupWidget).OnClose = value;
        }
        public bool Interrupt => (InnerWidget as IPopupWidget).Interrupt;

        public override bool Render(bool forceUpdate)
        {
            forceUpdate |= Dirty;

            forceUpdate |= InnerWidget.Render(forceUpdate);
            if (forceUpdate)
            {
                RenderBorders();
                Dirty = false;
            }

            return forceUpdate;
        }

        protected void RenderBorders() {
            
            for (int x = Size.Left + 1; x < Size.Right - 1; x++)
            {
                var c = new Symbol((char)(205), FcColor, BgColor);
                WindowData.Data[x, Size.Top] = c;
                WindowData.Data[x, Size.Bottom - 1] = c;
            }

            for (int y = Size.Top + 1; y < Size.Bottom - 1; y++)
            {
                var c = new Symbol((char)(186), FcColor, BgColor);
                WindowData.Data[Size.Left, y] = c;
                WindowData.Data[Size.Right - 1, y] = c;
            }

            {
                WindowData.Data[Size.Left, Size.Top] = new Symbol((char)(201), FcColor, BgColor);
                WindowData.Data[Size.Left, Size.Bottom - 1] = new Symbol((char)(200), FcColor, BgColor);
                WindowData.Data[Size.Right - 1, Size.Top] = new Symbol((char)(187), FcColor, BgColor);
                WindowData.Data[Size.Right - 1, Size.Bottom - 1] = new Symbol((char)(188), FcColor, BgColor);
            }
        }

        protected override void RenderImpl() { throw new Exception("Function should never be called"); }

        protected override void OnResized()
        {
            var free = Size.Grow(-1);
            InnerWidget.Resize(ref free);
        }

        public override Widget WidgetAt(Vec pos)
        {
            if (InnerWidget.Size.Contains(pos))
                return InnerWidget.WidgetAt(pos);

            return this;
        }

        public void OnKeyPress(char keyCode, EventFlags flags) => (InnerWidget as IKeyEventSuscriber)?.OnKeyPress(keyCode, flags);
        public void OnArrowPress(Vec move, EventFlags flags) => (InnerWidget as IKeyEventSuscriber)?.OnArrowPress(move, flags);

        public void OnMouseEvent(Vec mousePos, EventFlags flags) => (InnerWidget as IMouseEventSuscriber)?.OnMouseEvent(mousePos, flags);
        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags) => (InnerWidget as IMouseEventSuscriber)?.OnMouseMove(mousePos, mouseMove, flags);
        public void OnMouseWheel(Vec delta, EventFlags flags) => (InnerWidget as IMouseEventSuscriber)?.OnMouseWheel(delta, flags);
    }
}
