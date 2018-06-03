using System;
using System.Collections.Generic;
using HackConsole;

namespace SurvivalHack.Ui
{
    internal class OptionWidget<T> : TextWidget, IKeyEventSuscriber, IMouseEventSuscriber, IPopupWidget
    {
        public Action<T> OnSelect;
        public List<T> Set;
        public string Question;
        private int _selectedIndex = 0;

        public Action OnClose { get; set; }
        public bool Interrupt => false; // Maybe, this should be an option?

        protected override void MakeLines()
        {
            WordWrap(Question, "", Color.White);

            var digits = Set.Count.ToString().Length;

            for (var i = 0; i < Set.Count; i++)
            {
                var c = (i < 26) ? (i + 'a') : (i - 26 + 'A') ;
                var prefix = $"{(char)c}:";
                var color = (i == _selectedIndex) ? Color.White : Color.Gray;
                WordWrap(Set[i].ToString(), prefix, color);
            }
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            if (keyCode == (char) 13) // Enter
            {
                Use(_selectedIndex);

                OnClose?.Invoke();
            }
            else if (keyCode == (char) 27) // Escape
            {
                OnClose?.Invoke();
            }
            else if (keyCode >= 'a' && keyCode <= 'z')
            {
                if (Use(keyCode - 'a'))
                    OnClose?.Invoke();
            }
            else if (keyCode >= 'A' && keyCode <= 'Z')
            {
                if (Use(keyCode - 'A' + 26))
                    OnClose?.Invoke();
            }
        }

        private bool Use(int index) {
            if (index >= 0 && index < Set.Count)
            {
                OnSelect?.Invoke(Set[index]);
                return true;
            }
            return false;
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            _selectedIndex += move.Y;
            if (_selectedIndex < 0)
                _selectedIndex = Math.Max(Set.Count - 1, 0);
            if (_selectedIndex >= Set.Count)
                _selectedIndex = 0;

            Lines.Clear();
            MakeLines();
        }


        public override void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            if (flags.HasFlag(EventFlags.LeftButton) && flags.HasFlag(EventFlags.MouseEventPress))
                if (Use(_selectedIndex))
                    OnClose?.Invoke();
        }

        public override void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            var relMousePos = mousePos - Size.TopLeft;
            _selectedIndex = relMousePos.Y - 1;

            Lines.Clear();
            MakeLines();

            Dirty = true;
        }
    }
}
