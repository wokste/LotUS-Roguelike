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
        public int Dir;
        public Entity Dest;

        public Stairs(Entity dest)
        {
            Dest = dest;
        }

        public string Describe() => $"Can be used to travel {(Dir == '>' ? "down" : "up")}";

        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent msg, EUseSource source)
        {
            if (msg is UpDownEvent && source == EUseSource.User)
            {
                var travelMsg = msg as UpDownEvent;
                if (travelMsg.Dir == Dir)
                    yield return new UseFunc(Travel);
                //message.OnEvent += Move; // TODO
            }
        }

        private void Travel(BaseEvent msg)
        {
            msg.User.Move.Unbind(msg.User);
            MoveComponent.Bind(msg.User, Dest.Move.Level, Dest.Move.Pos);
        }

        public static void Link(Level srcMap, Vec srcPos, Level destMap, Vec destPos)
        {
            var e1 = new Entity('>', "stairs down", EEntityFlag.FixedPos);
            var e2 = new Entity('<', "stairs up", EEntityFlag.FixedPos);
            MoveComponent.Bind(e1, srcMap, srcPos);
            MoveComponent.Bind(e2, destMap, destPos);
            e1.Add(new Stairs(e2));
            e2.Add(new Stairs(e1));
        }
    }

    public class UpDownEvent : BaseEvent
    {
        public char Dir;
        public string Method = "";

        public UpDownEvent(Entity user, Entity stairs, char dir) : base(user, stairs, null)
        {
            Debug.Assert(dir == '>' || dir == '<');

            Dir = dir;
        }

        public override string GetMessage(bool isChildMessage)
        {
            return $"{Word.AName(User)} {Word.Verb(User, "walk")} {(Dir == '>' ? "down" : "up")} the {Method}. ";
        }
    }
}
