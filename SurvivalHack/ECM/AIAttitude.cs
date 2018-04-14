using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.ECM
{
    public class AiAttitude
    {
        public Entity FindEnemy(Entity self)
        {
            var pos = self.Move.Pos;

            // Todo: Sight radius
            var area = new Rect(pos - new Vec(10, 10), new Vec(21, 21));

            foreach (var e in self.Move.Level.GetEntities(area))
            {
                if (e == self)
                    continue;

                if (AttitudeSee(self, e) == EAttitude.Ignore)
                    continue;

                var delta = self.Move.Pos - e.Move.Pos;

                if (delta.LengthSquared < 100) // Todo: Sight radius
                    return e;
            }
            return null;
        }

        private EAttitude AttitudeSee(Entity self, Entity other)
        {
            return (other is Player) ? EAttitude.Hate : EAttitude.Ignore;
        }
    }

    public enum EAttitude
    {
        Ignore, // Ignores other character
        Follow, // Follow it
        Hate,   // Attacks it
        Fear,   // Run away
    }
}
