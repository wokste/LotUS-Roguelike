using HackConsole;

namespace SurvivalHack.ECM
{
    public class Stairs : IActionComponent
    {
        public int Depth;

        public Stairs(int depth)
        {
            Depth = depth;
        }


        public void GetActions(Entity self, BaseEvent msg, EUseSource source)
        {
            if (msg is DownEvent && source == EUseSource.Item)
            {
                msg.OnEvent += Travel;
            }
        }

        private void Travel(BaseEvent msg)
        {
            Game game = msg.User.Level.Game;
            (var Level, var Pos) = game.GetLevel(Depth);

            msg.User.SetLevel(Level, Pos);
        }

        public static void MakeStairs(Level srcMap, Vec srcPos, int depth)
        {
            var e1 = new Entity(new TileGlyph(3,9), "stairs down", EEntityFlag.FixedPos);
            e1.SetLevel(srcMap, srcPos);
            e1.Add(new Stairs(depth));
        }

        public bool FitsIn(ESlotType type) => false;
    }
}
