using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole;

namespace SurvivalHack.Ui
{
    class CharacterWidget : Widget
    {
        private readonly Creature _creature;

        private static readonly char[] Gradient = new[] {(char) 0xC2, (char) 0xC1, (char) 0xC0, ' '};

        public CharacterWidget(Creature creature)
        {
            _creature = creature;
        }

        void PrintBar(string name, int y, Bar bar, Color fgColor, Color bgColor = default(Color))
        {
            Print(0, y, name, Color.White, Color.Transparent);

            var totalLen = Size.Width - name.Length;
            var fillLen = totalLen * bar.Perc;
            var fillLenInt = (int) fillLen;
            var lastChar = Gradient[(int)((fillLen - fillLenInt) * Gradient.Length)];
            
            var sb = new StringBuilder(totalLen);
            sb.Append((char)0xDB, fillLenInt);
            if (fillLenInt < totalLen)
                sb.Append(lastChar);
            sb.Append(' ', totalLen - sb.Length);

            Print(name.Length, y, sb.ToString(), fgColor, bgColor);
        }

        public override void Render(bool forceUpdate)
        {
            Clear();

            var y = 0;

            PrintBar("HP:", y++, _creature.Health, Color.Green, Color.Red);
            PrintBar("NU:", y++, _creature.Hunger, Color.Yellow, Color.Red);
            y++;
            Print(0, y++, "Inventory", Color.White);

            foreach (var inv in _creature.Inventory._items)
            {
                Print(0, y++, $"- {inv}", Color.Gray);
            }

        }
    }
}
