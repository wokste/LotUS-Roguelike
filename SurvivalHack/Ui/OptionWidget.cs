using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole;

namespace SurvivalHack.Ui
{
    class OptionWidget<T> : TextWidget, IKeyEventSuscriber
    {
        public Action<T> OnSelect;
        public Action OnAbort;
        public List<T> Set;
        public string Question;

        private int _selectedIndex = 0;
        
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
                OnSelect?.Invoke(Set[_selectedIndex]);
            }

            if (keyCode == (char) 27) // Escape
            {
                OnAbort?.Invoke();
            }
        }
        
        public void OnArrowPress(Vec move, EventFlags flags)
        {
            _selectedIndex += move.Y;
            while (_selectedIndex < 0)
                _selectedIndex += Set.Count;
            while (_selectedIndex >= Set.Count)
                _selectedIndex -= Set.Count;

            MakeLines();
        }
    }
}
