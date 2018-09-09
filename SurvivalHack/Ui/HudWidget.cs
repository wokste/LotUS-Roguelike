using HackConsole;
using System.Linq;

namespace SurvivalHack.Ui
{
    public class HudWidget : GridWidget
    {
        private readonly TurnController _controller;

        private static readonly char[] Gradient = new[] {' ', (char) 0xB0, (char) 0xB1, (char) 0xB2};

        public HudWidget(TurnController controller)
        {
            _controller = controller;

            controller.OnTurnEnd += () => Dirty = true;
        }

        private void PrintBar(string name, int y, Bar bar)
        {
            var str = $"{name} ({bar.Current}/{bar.Max})";
            var width = Data.Size.X;
            var offset = (width - str.Length) / 2;

            var p = bar.Perc;
            var fgColor = Colour.White;// (p > 0.8) ? Color.Green : (p > 0.5) ? Color.Yellow : (p > 0.2) ? Color.Orange : Color.Red;
            
            for (int x = 0; x < width; x++)
            {
                var bgColor = (x <= p * width + 0.5) ? Colour.Red : Colour.Black;

                var ascii = (x >= offset && x < str.Length + offset) ? str[x-offset] : ' ';
                Data[new Vec(x, y)] = new Symbol { Ascii = ascii, BackgroundColor = bgColor, TextColor = fgColor };
            }

        }

        protected override void Render()
        {
            Clear(Colour.Red);

            var y = 0;

            Print(new Vec(0, y++), "Player:", Colour.White, Colour.Transparent);

            if (_controller.GameOver)
            {
                Print(new Vec(0, y++), "Dead", Colour.White, Colour.Transparent);
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

                Print(new Vec(0, y++), e.Name, Colour.White, Colour.Transparent);

                var damagable = e.GetOne<Combat.Damagable>();
                if (damagable != null)
                    PrintBar("HP:", y++, damagable.Health);
            }
        }
    }
}
