using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    static class Word
    {
        public static string Name(Entity e)
        {
            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? "you" : e.Name;
        }

        public static string AName(Entity e)
        {
            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? "you" : $"a {e.Name}";
        }

        public static string It(Entity e)
        {
            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? "you" : "it";
        }

        public static string Its(Entity e)
        {
            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? "your" : "its";
        }

        public static string Verb(Entity e, string verb, string verbs = null)
        {
            if (verbs == null)
                verbs = verb + "s";

            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? verb : verbs;
        }
    }
}
