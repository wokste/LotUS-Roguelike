using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole
{
    class PopupStack
    {
        private readonly List<Widget> _widgets = new List<Widget>();

        public void Render(bool forceUpdate)
        {
            // TODO: Some widgets may be hidden below other widgets which allows optimalization.

            foreach (var w in _widgets)
                w.Render(forceUpdate);
        }
        
        public void Push(Widget w)
        {
            (w as IPopupWidget).OnClose += () => { Pop(w); };
            _widgets.Add(w);
        }

        private void Pop(Widget w)
        {
            _widgets.Remove(w);
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
