using System;
using System.Collections.Generic;

namespace HackConsole
{
    public class WidgetContainer : Widget
    {
        public List<Widget> Widgets = new List<Widget>();

        public override bool Render(bool forceUpdate)
        {
            var rendered = false;
            foreach (var w in Widgets)
                rendered |= w.Render(forceUpdate);

            return rendered;
        }

        protected override void RenderImpl() { throw new Exception("Function should never be called"); }

        public void Add(Widget w)
        {
            Widgets.Add(w);
            OnResized();
        }

        protected override void OnResized()
        {
            var free = Size;
            foreach (var w in Widgets)
            {
                w.Resize(ref free);
            }
        }

        public override Widget WidgetAt(Vec pos)
        {
            foreach (var w in Widgets)
                if (w.Size.Contains(pos))
                    return w.WidgetAt(pos);

            return null;
        }
    }
}
