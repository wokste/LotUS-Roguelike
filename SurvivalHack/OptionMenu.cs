using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class OptionMenu
    {
        private readonly string _question;
        private readonly List<Option> _options = new List<Option>();

        public OptionMenu(string question)
        {
            _question = question;
        }

        public void Add(string name, Action action)
        {
            _options.Add(new Option
            {
                Name = name,
                Action = action
            });
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine(_question);
                var i = 1;

                foreach (var o in _options)
                {
                    Console.WriteLine($"{i}. {o.Name}");
                    i++;
                }

                var keyStr = Console.ReadLine();
                int index;

                if (!int.TryParse(keyStr, out index))
                {
                    Console.WriteLine("Didn't understand answer");
                    continue;
                }

                index--;

                if (index < 0 || index >= _options.Count)
                {
                    Console.WriteLine("Out of bounds");
                    continue;
                }

                _options[index].Action();

                return;
            }
        }

        private class Option
        {
            public string Name;
            public Action Action;
        }
    }
}
