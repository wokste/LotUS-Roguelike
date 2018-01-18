﻿using System;
using System.Collections.Generic;

namespace HackConsole
{
    public class WidgetContainer : Widget
    {
        public List<Widget> Widgets = new List<Widget>();

        public override void Render()
        {
            foreach (var w in Widgets)
            {
                w.Render();
            }
        }

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
    }
}