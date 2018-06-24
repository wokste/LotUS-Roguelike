using System;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack.Ui
{
    public class OptionWidget : Widget, IKeyEventSuscriber, IMouseEventSuscriber, IPopupWidget
    {
        public Action<Entity> OnSelect;
        public List<Entity> Set;
        public string Question;
        private int _selectedIndex = 0;
        //private int _columns = 1;
        private const int COLUMN_WIDTH = 42;

        public Action OnClose { get; set; }
        public bool Interrupt => false; // Maybe, this should be an option?

        public OptionWidget(string question, List<Entity> set, Action<Entity> onSelect)
        {
            Question = question;
            Set = set;
            OnSelect = onSelect;
            //_columns = 1;

            DesiredSize = new Rect(0, 0, COLUMN_WIDTH, Set.Count + 1);
        }

        protected override void RenderImpl()
        {
            Clear();

            Print(new Vec(0, 0), Question, Color.White);

            for (var i = 0; i < Set.Count; i++)
            {
                var y = i + 1;
                var item = Set[i];

                var color = (i == _selectedIndex) ? Color.White : Color.Gray;

                if (item == null)
                {
                    Print(new Vec(2, y), "None", color);
                }
                else
                {
                    Print(new Vec(0, y), item.Symbol);
                    Print(new Vec(2, y), item.Name, color);
                }
            }
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            _selectedIndex += move.Y;
            if (_selectedIndex < 0)
                _selectedIndex = Math.Max(Set.Count - 1, 0);
            if (_selectedIndex >= Set.Count)
                _selectedIndex = 0;

            Dirty = true;
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            if (keyCode == (char)13) // Enter
            {
                Use();

                OnClose?.Invoke();
            }
            else if (keyCode == (char)27) // Escape
            {
                OnClose?.Invoke();
            }
        }

        private bool Use()
        {
            if (_selectedIndex >= 0 && _selectedIndex < Set.Count)
            {
                OnSelect?.Invoke(Set[_selectedIndex]);
                return true;
            }
            return false;
        }

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            if (flags.HasFlag(EventFlags.LeftButton) && flags.HasFlag(EventFlags.MouseEventRelease))
                if (Use())
                    OnClose?.Invoke();
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            var relMousePos = mousePos - Size.TopLeft;
            var index = relMousePos.Y - 1;
            if (index >= 0 && index < Set.Count && _selectedIndex != index)
            {
                _selectedIndex = index;
                Dirty = true;
            }
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
