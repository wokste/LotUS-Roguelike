using HackConsole;
using SurvivalHack.Combat;
using SFML.Graphics;

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

        private void PrintBar(string name, int y, StatBlock stats, int statID)
        {
            var str = $"{name} ({stats.Cur(statID)}/{stats.Max(statID)})";
            var width = Data.Size.X;
            var offset = (width - str.Length) / 2;

            var p = stats.Perc(statID);
            var fgColor = Color.White;// (p > 0.8) ? Color.Green : (p > 0.5) ? Color.Yellow : (p > 0.2) ? Color.Orange : Color.Red;
            
            for (int x = 0; x < width; x++)
            {
                var bgColor = (x <= p * width + 0.5) ? Color.Red : Color.Black;

                var ascii = (x >= offset && x < str.Length + offset) ? str[x-offset] : ' ';
                Data[new Vec(x, y)] = new Symbol { Ascii = ascii, BackgroundColor = bgColor, TextColor = fgColor };
            }

        }

        protected override void Render()
        {
            Clear(Color.Red);

            var y = 0;

            Print(new Vec(0, y++), "Player:", Color.White, Color.Transparent);

            if (_controller.GameOver)
            {
                Print(new Vec(0, y++), "Dead", Color.White, Color.Transparent);
                return;
            }
            var statblock = _controller.Player.GetOne<StatBlock>();
            PrintBar("HP:", y++, statblock, 0);
            PrintBar("MP:", y++, statblock, 1);
            PrintBar("XP:", y++, statblock, 2);
            
            foreach (var e in _controller.VisibleEnemies)
            {
                y++;
                var fgColor = (e == _controller.SelectedTarget) ? Color.White : new Color(128,128,128);

                Print(new Vec(0, y++), e.Name, fgColor, Color.Transparent);


                var damagable = e.GetOne<StatBlock>();
                if (damagable != null)
                    PrintBar("HP:", y++, damagable, 0);
            }
            Dirty = true; // This is temporary.
        }
    }
}
