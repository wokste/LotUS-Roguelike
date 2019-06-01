using SurvivalHack.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Effects
{
    public class Heal : IEntityEffect
    {
        public int Restore;
        public int StatID;

        public EntityTarget Target { get; }

        public Heal(int restore, int statID, EntityTarget target)
        {
            Restore = restore;
            StatID = statID;
            Target = target;
        }

        public string Describe() => $"Heals {Restore} when used.";

        public bool Use(Entity instignator, Entity target, StringBuilder sb)
        {
            var stats = target.GetOne<StatBlock>();

            stats.Heal(Restore, StatID);
            sb.Append($"{Word.AName(target)} {Word.Verb(target, "heal")} {Restore} HP. "); // TODO: not always HP
            return true;
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1; // TODO
        }
    }
}
