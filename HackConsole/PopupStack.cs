using System;
using System.Collections.Generic;

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

        public override bool Render(bool forceUpdate)
        {
            // TODO: Some widgets may be hidden below other widgets which allows optimalization.

            var rendered = false;
            foreach (var w in _widgets)
                if (w.Render(rendered || forceUpdate))
                    rendered = true; // All widgets on top of this widget should be forced to update. In addition we rendered 

            return rendered;
        }

        protected override void RenderImpl() { throw new Exception("Function should never be called"); }

        protected override void OnResized()
        {
            foreach (var w in _widgets)
            {
                w.CenterPopup(Size);
            }
        }

        public void Push(Widget innerWidget)
        {
            var outerWidget = new BorderedWidget(innerWidget);
            outerWidget.CenterPopup(Size);
            (innerWidget as IPopupWidget).OnClose += () => { Pop(outerWidget); };
            _widgets.Add(outerWidget);
        }

        private void Pop(Widget w)
        {
            _widgets.Remove(w);
            WindowData.ForceUpdate = true;
        }

        public override Widget WidgetAt(Vec v)
        {
            for (int i = _widgets.Count - 1; i >= 0; i--)
            {
                var w = _widgets[i];
                if (w.Size.Contains(v))
                    return w.WidgetAt(v);
            }

            return null;
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
