using System.Text;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Ui
{
    internal class EntityDetailWidget : Widget
    {
        private readonly Entity _entity;

        private static readonly char[] Gradient = new[] {' ', (char) 0xB0, (char) 0xB1, (char) 0xB2};

        public EntityDetailWidget(Entity entity)
        {
            _entity = entity;
        }

        private void PrintBar(string name, int y, Bar bar, Color fgColor, Color bgColor = default(Color))
        {
            var str = $"{name} ({bar.Current}/{bar.Max})";

            Print(new Vec(0, y), str, fgColor, Color.Transparent);
            /*
            Print(0, y, name, Color.White, Color.Transparent);

            var totalLen = Area.Width - name.Length;
            var fillLen = totalLen * bar.Perc;
            var fillLenInt = (int) fillLen;
            var lastChar = Gradient[(int)((fillLen - fillLenInt) * Gradient.Length)];
            
            var sb = new StringBuilder(totalLen);
            sb.Append((char)0xDB, fillLenInt);
            if (fillLenInt < totalLen)
                sb.Append(lastChar);
            sb.Append(' ', totalLen - sb.Length);

            Print(name.Length, y, sb.ToString(), fgColor, bgColor);*/
        }

        public override void Render(bool forceUpdate)
        {
            Clear();

            var y = 0;

            PrintBar("HP:", y++, _entity.Health, Color.Green, Color.Red);
            PrintBar("NU:", y++, _entity.Hunger, Color.Yellow, Color.Red);
            y++;
            Print(new Vec(0, y++), "Inventory", Color.White);

            foreach (var inv in _entity.Inventory._items)
            {
                Print(new Vec(0, y++), $"- {inv}", Color.Gray);
            }

        }
    }
}
