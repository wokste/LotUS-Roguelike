using HackConsole;
using System.Linq;

namespace SurvivalHack.Ui
{
    public class HudWidget : Widget
    {
        private readonly TurnController _controller;

        private static readonly char[] Gradient = new[] {' ', (char) 0xB0, (char) 0xB1, (char) 0xB2};

        public HudWidget(TurnController controller)
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

            Print(new Vec(0, y++), "Player:", Color.White, Color.Transparent);

            if (_controller.Player == null)
            {
                Print(new Vec(0, y++), "Dead", Color.White, Color.Transparent);
                return;
            }
            PrintBar("HP:", y++, _controller.Player.GetOne<Combat.Damagable>().Health);

            var FoV = _controller.FoV;
            var monsterList = _controller.Level.GetEntities().Where(e => (
                e != _controller.Player &&
                e.EntityFlags.HasFlag(EEntityFlag.TeamMonster) &&
                FoV.ShowLocation(e) != null
            ));

            foreach (var e in monsterList)
            {
                y++;

                Print(new Vec(0, y++), e.Name, Color.White, Color.Transparent);

                var damagable = e.GetOne<Combat.Damagable>();
                if (damagable != null)
                    PrintBar("HP:", y++, damagable.Health);
            }
        }
    }
}
