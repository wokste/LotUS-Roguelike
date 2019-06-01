using SurvivalHack.ECM;
using System;
using System.Linq;

namespace SurvivalHack.Factory
{
    public class PotionFactory : IEntityFactory
    {
        static int BaseId;
        readonly (byte, string, string)[] types = new(byte, string, string)[]{
            (0, "a", "red"),
            (1, "a", "pink"),
            (2, "an", "orange"),
            (3, "a", "yellow"),
            (4, "a", "grassy"),
            (5, "a", "mouldy"),
            (6, "a", "cyan"),
            (7, "a", "light blue"),
            (8, "a", "dark blue"),
            (16, "an", "oily"),
            (17, "a", "watery"),
            (18, "a", "black"),
            (19, "a", "golden"),
            (20, "a", "brown"),
            (22, "a", "gray"),
            (23, "a", "white"),

        };

        public PotionFactory(Random rnd)
        {
            types = types.OrderBy(p => rnd.Next()).ToArray();
            BaseId = StackComponent.GenMergeId(types.Length);
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            var potionId = info.Rnd.Next(3);

            (var x, var article, var colorName) = types[potionId];


            Entity e = new Entity(new TileGlyph(x, 15), $"{colorName} potion", EEntityFlag.Pickable | EEntityFlag.Consumable | EEntityFlag.Throwable);

            e.Add(new StackComponent(1, potionId + BaseId));

            switch (potionId)
            {
                case 0: // Lesser healing potion
                    e.Add(new HealComponent(20, 0, typeof(ConsumeEvent)));
                    break;
                case 1: // Greater healing potion
                    e.Add(new HealComponent(40, 0, typeof(ConsumeEvent)));
                    break;
                case 2: // Poison draught
                    //e.Add(new Combat.Poisonous(20, typeof(ConsumeEvent)));
                    break;
            }
            //TODO: Add randomized effects to the potion.

            //TODO: Work on identitying potions

            return e;
        }
    }
}
