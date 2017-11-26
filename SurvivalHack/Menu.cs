using System;
using System.Collections.Generic;

namespace SurvivalHack
{
    static class Menu
    {
        public static T ShowList<T>(string question, IList<T> options)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(question);
                var i = 1;
                
                foreach (var o in options)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{i}. {o}");
                    i++;
                }

                Console.ForegroundColor = ConsoleColor.White;
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
                return options[index];
            }
        }

        public static int AskInt(string question, int min, int max)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(question);
                
                var valStr = Console.ReadLine();
                if (valStr == "")
                    return 0;

                if (!int.TryParse(valStr, out var val))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Didn't understand answer");
                    continue;
                }

                return Math.Min(Math.Max(0, val), max);
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
            var option = Menu.ShowList(_question, _options);
            option?.Action();
        }

        private class Option
        {
            public string Name;
            public Action Action;

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
