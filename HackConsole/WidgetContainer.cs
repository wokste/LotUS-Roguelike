using System.Collections.Generic;
using System.Diagnostics;
using SFML.Graphics;

namespace HackConsole
{
    public class WidgetContainer : Widget
    {
        public List<Widget> Widgets = new List<Widget>();

        public void Add(Widget w)
        {
            Debug.Assert(w.Parent == null);

            Widgets.Add(w);
            w.Parent = this;
            OnResized();
        }

        protected override void OnResized()
        {
            var free = Rect;
            foreach (var w in Widgets)
            {
                w.ResizeDocked(ref free);
            }
        }

        public override Widget WidgetAt(Vec pos)
        {
            foreach (var w in Widgets)
                if (w.Rect.Contains(pos))
                    return w.WidgetAt(pos);

            return null;
        }

        protected override void DrawInternal(RenderTarget target)
        {

            foreach (var w in Widgets)
                w.Draw(target);
        }
    }
}
