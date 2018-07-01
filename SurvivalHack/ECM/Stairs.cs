using HackConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.ECM
{
    public class Stairs : IComponent
    {
        public char Ascii;
        public Entity Dest;

        public Stairs(Entity dest, char ascii)
        {
            Dest = dest;
            Ascii = ascii;
        }

        public string Describe() => $"Can be used to travel {(Ascii == '>' ? "down" : "up")}";


        public void GetActions(Entity self, BaseEvent msg, EUseSource source)
        {
            if (msg is UpDownEvent && source == EUseSource.Item)
            {
                var travelMsg = msg as UpDownEvent;
                if (travelMsg.Dir == Ascii)
                    msg.OnEvent += Travel;
            }
        }

        private void Travel(BaseEvent msg)
        {
            MoveComponent.Bind(msg.User, Dest.Move.Level, Dest.Move.Pos);
        }

        public static void Link(Level srcMap, Vec srcPos, Level destMap, Vec destPos)
        {
            var e1 = new Entity('>', "stairs down", EEntityFlag.FixedPos);
            var e2 = new Entity('<', "stairs up", EEntityFlag.FixedPos);
            MoveComponent.Bind(e1, srcMap, srcPos);
            MoveComponent.Bind(e2, destMap, destPos);
            e1.Add(new Stairs(e2, '>'));
            e2.Add(new Stairs(e1, '<'));
        }

    }
}
