using System;
using SFML.Graphics;

namespace HackConsole
{
    public class BorderedWidget : Widget, IPopupWidget, IKeyEventSuscriber
    {
        readonly Widget InnerWidget;

        public int BorderWidth = 4;

        public BorderedWidget(Widget inner) {
            InnerWidget = inner;
            DesiredSize = InnerWidget.DesiredSize.Grow(BorderWidth);
            Parent = inner;
        }

        public Action OnClose {
            get => (InnerWidget as IPopupWidget).OnClose;
            set => (InnerWidget as IPopupWidget).OnClose = value;
        }
        public bool Interrupt => (InnerWidget as IPopupWidget).Interrupt;

        protected override void OnResized()
        {
            var free = Rect.Grow(-BorderWidth);
            InnerWidget.Resize(free);
        }

        public override Widget WidgetAt(Vec pos)
        {
            if (InnerWidget.Rect.Contains(pos))
                return InnerWidget.WidgetAt(pos);

            return this;
        }

        public void OnKeyPress(char keyCode, EventFlags flags) => (InnerWidget as IKeyEventSuscriber)?.OnKeyPress(keyCode, flags);
        public void OnArrowPress(Vec move, EventFlags flags) => (InnerWidget as IKeyEventSuscriber)?.OnArrowPress(move, flags);

        protected override void DrawInternal(RenderTarget target)
        {
            // TODO: render border
            InnerWidget.Draw(target);
        }
    }
}
