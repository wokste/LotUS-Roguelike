using HackConsole;

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

        private void PrintBar(string name, int y, Bar bar)
        {
            var str = $"{name} ({bar.Current}/{bar.Max})";

            var p = bar.Perc;
            var fgColor = (p > 0.8) ? Color.Green : (p > 0.5) ? Color.Yellow : (p > 0.2) ? Color.Orange : Color.Red;

            Print(new Vec(0, y), str, fgColor, Color.Transparent);
        }

        protected override void RenderImpl()
        {
            Clear();

            var y = 0;

            PrintBar("HP:", y++, _entity.Health);
            PrintBar("Food:", y++, _entity.Hunger);
            y++;
            Print(new Vec(0, y++), "Inventory", Color.White);

            foreach (var inv in _entity.GetOne<Inventory>()._items)
            {
                Print(new Vec(0, y++), $"- {inv}", Color.Gray);
            }
        }
    }
}
