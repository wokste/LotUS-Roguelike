using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Ui
{
    internal class EntityDetailWidget : Widget
    {
        private readonly TurnController _controller;

        private static readonly char[] Gradient = new[] {' ', (char) 0xB0, (char) 0xB1, (char) 0xB2};

        public EntityDetailWidget(TurnController controller)
        {
            _controller = controller;

            controller.OnTurnEnd += RenderImpl;
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

            PrintBar("HP:", y++, _controller.Player.GetOne<Combat.Damagable>().Health);

            y++;
            Print(new Vec(0, y++), "Inventory", Color.White);

            foreach (var inv in _controller.Player.GetOne<Inventory>().Items)
            {
                Print(new Vec(0, y++), $"- {inv}", Color.Gray);
            }
        }
    }
}
