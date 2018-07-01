using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Linq;

namespace SurvivalHack.Factory
{
    public class PotionFactory : IEntityFactory
    {
        static int BaseId;

        (string, string, Color)[] types = new(string, string, Color)[]{
            ("a", "red", Color.Parse("#f33")),
            ("a", "blue", Color.Parse("#36f")),
            ("an", "aqua", Color.Parse("#0fc")),
            ("a", "purple", Color.Parse("#f0f")),
            ("a", "green", Color.Parse("#3f0")),
            ("a", "violet", Color.Parse("#CDA4DE")),
            ("a", "yellow", Color.Parse("#FFE135")),
            ("a", "bronze ", Color.Parse("#CD7F32")),
            ("an", "orange ", Color.Parse("#ED9121")),
            ("a", "crystal ", Color.Parse("#A7D8DE")),
            ("a", "pink ", Color.Parse("#FFC0CB")),
            ("a", "sandy ", Color.Parse("#EDC9AF")),
            ("a", "beige ", Color.Parse("#F5F5DC")),
            ("a", "gold ", Color.Parse("#CFB53B")),
            ("a", "mossy ", Color.Parse("#867E36")),
            ("a", "rusty ", Color.Parse("#B7410E")),
            ("a", "silvery ", Color.Parse("#848482")),
            ("a", "peachy ", Color.Parse("#FFE5B4")),
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
