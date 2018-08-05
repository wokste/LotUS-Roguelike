using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace HackConsole
{
    public class WidgetContainer : Widget
    {
        public List<Widget> Widgets = new List<Widget>();

        public void Add(Widget w)
        {
            Widgets.Add(w);
            OnResized();
        }

        protected override void OnResized()
        {
            var free = Rect;
            foreach (var w in Widgets)
            {
                w.Resize(ref free);
            }
        }

        public override Widget WidgetAt(Vec pos)
        {
            foreach (var w in Widgets)
                if (w.Rect.Contains(pos))
                    return w.WidgetAt(pos);

            return null;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {

            foreach (var w in Widgets)
                w.Draw(target, states);
        }
    }
}
