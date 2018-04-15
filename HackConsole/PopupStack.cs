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

        public override void Render(bool forceUpdate)
        {
            // TODO: Some widgets may be hidden below other widgets which allows optimalization.

            foreach (var w in _widgets)
                w.Render(forceUpdate);
        }

        protected override void OnResized()
        {
            foreach (var w in _widgets)
            {
                w.CenterPopup(Size);
            }
        }

        public void Push(Widget w)
        {
            w.CenterPopup(Size);
            (w as IPopupWidget).OnClose += () => { Pop(w); };
            _widgets.Add(w);
        }

        private void Pop(Widget w)
        {
            _widgets.Remove(w);
        }

        public Widget WidgetAt(Vec v)
        {
            for (int i = _widgets.Count - 1; i >= 0; i--)
            {
                var w = _widgets[i];
                if (w.Size.Contains(v))
                    return w;
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
