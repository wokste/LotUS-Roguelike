using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole;

namespace SurvivalHack.Ui
{
    internal class OptionWidget<T> : TextWidget, IKeyEventSuscriber, IPopupWidget
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
                var prefix = i.ToString().PadLeft(digits) + ":";
                var color = i == _selectedIndex ? Color.White : Color.Gray;
                WordWrap(Set[i].ToString(), prefix, Color.Gray);
            }
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            if (keyCode == (char) 13) // Enter
            {
                if (Set.Count != 0)
                    OnSelect?.Invoke(Set[_selectedIndex]);

                OnClose?.Invoke();
            }

            if (keyCode == (char) 27) // Escape
            {
                OnClose?.Invoke();
            }
        }
        
        public void OnArrowPress(Vec move, EventFlags flags)
        {
            _selectedIndex += move.Y;
            if (_selectedIndex < 0)
                _selectedIndex = Math.Max(Set.Count - 1, 0);
            if (_selectedIndex >= Set.Count)
                _selectedIndex = 0;

            MakeLines();
        }
    }
}
