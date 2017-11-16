using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    static class Menu
    {
        public static T ShowList<T>(string question, List<T> options, Func<T,bool> canSelect)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(question);
                var i = 1;
                
                foreach (var o in options)
                {
                    Console.ForegroundColor = (canSelect(o) ? ConsoleColor.Yellow : ConsoleColor.DarkGray);
                    Console.WriteLine($"{i}. {o}");
                    i++;
                }

                var keyStr = Console.ReadLine();
                if (keyStr == "")
                    return default(T);

                if (!int.TryParse(keyStr, out var index))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Didn't understand answer");
                    continue;
                }

                index--;

                if (index < 0 || index >= options.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Out of bounds");
                    continue;
                }
                
                if (canSelect(options[index]))
                    return options[index];
            }
        }
    }

    class ActionMenu
    {
        private readonly string _question;
        private readonly List<Option> _options = new List<Option>();

        public ActionMenu(string question)
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
            var option = Menu.ShowList(_question, _options, o => true);
            option?.Action();
        }

        private class Option
        {
            internal string Name;
            internal Action Action;

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
