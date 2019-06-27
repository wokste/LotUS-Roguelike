namespace SurvivalHack
{
    static public class Word
    {
        public static string Name(Entity e)
        {
            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? "you" : $"{ColorString(e)}{e.Name}@ca";
        }

        public static string AName(Entity e)
        {
            return e.EntityFlags.HasFlag(EEntityFlag.IsPlayer) ? "you" : $"a {ColorString(e)}{e.Name}@ca";
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


        public static string ColorString(Entity e)
        {
            var f = e.EntityFlags;
            if (f.HasFlag(EEntityFlag.Item))
            {
                if (f.HasFlag(EEntityFlag.Identified) && f.HasFlag(EEntityFlag.Cursed))
                    return "@cd"; // Cursed items are red

                if (f.HasFlag(EEntityFlag.Consumable))
                    return "@ch"; // Consumable items are yellow

                // Items
                return "@cf"; // Items are gray
            }
            else if (f.HasFlag(EEntityFlag.TeamMonster)){
                return "@cd"; // Enemy monsters are red
            }
            else if (f.HasFlag(EEntityFlag.TeamPlayer))
            {
                return "@cg"; // Allied monsters are green
            }
            return "@cf"; // Just, dunno. Pick gray.
        }
    }
}
