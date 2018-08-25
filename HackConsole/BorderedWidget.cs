using System;
using SFML.Graphics;

namespace HackConsole
{
    public class BorderedWidget : Widget, IPopupWidget, IKeyEventSuscriber, IMouseEventSuscriber
    {
        Widget InnerWidget;
        Colour FcColor = Colour.White;
        Colour BgColor = Colour.Gray;

        public int BorderWidth = 4;

        public BorderedWidget(Widget inner) {
            InnerWidget = inner;
            DesiredSize = InnerWidget.DesiredSize.Grow(BorderWidth);
        }

        public Action OnClose {
            get => (InnerWidget as IPopupWidget).OnClose;
            set => (InnerWidget as IPopupWidget).OnClose = value;
        }
        public bool Interrupt => (InnerWidget as IPopupWidget).Interrupt;

        protected override void OnResized()
        {
            var free = Rect.Grow(-BorderWidth);
            InnerWidget.Resize(ref free);
        }

        public override Widget WidgetAt(Vec pos)
        {
            if (InnerWidget.Rect.Contains(pos))
                return InnerWidget.WidgetAt(pos);

            return this;
        }

        public void OnKeyPress(char keyCode, EventFlags flags) => (InnerWidget as IKeyEventSuscriber)?.OnKeyPress(keyCode, flags);
        public void OnArrowPress(Vec move, EventFlags flags) => (InnerWidget as IKeyEventSuscriber)?.OnArrowPress(move, flags);

        public void OnMouseEvent(Vec mousePos, EventFlags flags) => (InnerWidget as IMouseEventSuscriber)?.OnMouseEvent(mousePos, flags);
        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags) => (InnerWidget as IMouseEventSuscriber)?.OnMouseMove(mousePos, mouseMove, flags);
        public void OnMouseWheel(Vec delta, EventFlags flags) => (InnerWidget as IMouseEventSuscriber)?.OnMouseWheel(delta, flags);

        public override void Draw(RenderTarget target)
        {
            // TODO: render border
            InnerWidget.Draw(target);
        }
    }
}
