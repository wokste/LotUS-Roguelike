using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.Graphics;

namespace HackConsole
{
    public class PopupStack : Widget
    {
        public PopupStack()
        {
            Docking = Docking.Fill;
        }

        private readonly List<Widget> _widgets = new List<Widget>();
        public Widget Top => (_widgets.Count > 0 ? _widgets[_widgets.Count - 1] : null);
        public bool Empty => (_widgets.Count == 0);

        protected override void OnResized()
        {
            foreach (var w in _widgets)
            {
                w.CenterPopup(Rect);
            }
        }

        public void Push(Widget innerWidget)
        {
            Debug.Assert(innerWidget.Parent == null);

            var outerWidget = new BorderedWidget(innerWidget);
            outerWidget.CenterPopup(Rect);
            (innerWidget as IPopupWidget).OnClose += () => { Pop(outerWidget); };
            _widgets.Add(outerWidget);

            outerWidget.Parent = this;
        }

        private void Pop(Widget w)
        {
            Debug.Assert(w.Parent == this);

            _widgets.Remove(w);
            w.Parent = null;
        }

        public override Widget WidgetAt(Vec v)
        {
            for (int i = _widgets.Count - 1; i >= 0; i--)
            {
                var w = _widgets[i];
                if (w.Rect.Contains(v))
                    return w.WidgetAt(v);
            }

            return null;
        }

        protected override void DrawInternal(RenderTarget target)
        {
            foreach (var w in _widgets)
                w.Draw(target);
        }
    }

    public interface IPopupWidget
    {
        /// <summary>
        /// Called when the widget should be closed. Used by the WidgetStack to remove said widget.
        /// </summary>
        Action OnClose { get; set; }

        /// <summary>
        /// Whether the widget interrupts all input. In general, this should be true only if the game can't continue without this.
        /// </summary>
        bool Interrupt { get; }
    }
}
