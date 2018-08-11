using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Linq;

namespace SurvivalHack.Factory
{
    public class PotionFactory : IEntityFactory
    {
        static int BaseId;

        (string, string, Colour)[] types = new(string, string, Colour)[]{
            ("a", "red", Colour.Parse("#f33")),
            ("a", "blue", Colour.Parse("#36f")),
            ("an", "aqua", Colour.Parse("#0fc")),
            ("a", "purple", Colour.Parse("#f0f")),
            ("a", "green", Colour.Parse("#3f0")),
            ("a", "violet", Colour.Parse("#CDA4DE")),
            ("a", "yellow", Colour.Parse("#FFE135")),
            ("a", "bronze ", Colour.Parse("#CD7F32")),
            ("an", "orange ", Colour.Parse("#ED9121")),
            ("a", "crystal ", Colour.Parse("#A7D8DE")),
            ("a", "pink ", Colour.Parse("#FFC0CB")),
            ("a", "sandy ", Colour.Parse("#EDC9AF")),
            ("a", "beige ", Colour.Parse("#F5F5DC")),
            ("a", "gold ", Colour.Parse("#CFB53B")),
            ("a", "mossy ", Colour.Parse("#867E36")),
            ("a", "rusty ", Colour.Parse("#B7410E")),
            ("a", "silvery ", Colour.Parse("#848482")),
            ("a", "peachy ", Colour.Parse("#FFE5B4")),
        };

        public PotionFactory(Random rnd)
        {
            types = types.OrderBy(p => rnd.Next()).ToArray();
            BaseId = StackComponent.GenMergeId(types.Length);
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            Entity e = new Entity('!', "Potion", EEntityFlag.Pickable | EEntityFlag.Consumable | EEntityFlag.Throwable);

            var potionId = info.Rnd.Next(3);

            (var article, var colorName, var color) = types[potionId];
            e.Symbol = new Symbol('!', color);

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
            e.Name = $"{colorName} potion";

            return e;
        }
    }
}
