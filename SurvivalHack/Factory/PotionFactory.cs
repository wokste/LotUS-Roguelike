using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Linq;

namespace SurvivalHack.Factory
{
    class PotionFactory : IEntityFactory
    {
        static int BaseId;

        (string, Color)[] types = new(string, Color)[]{
            ("Red", Color.Parse("#f33")),
            ("blue", Color.Parse("#36f")),
            ("aqua", Color.Parse("#0fc")),
            ("yellow", Color.Parse("#cf0")),
            ("purple", Color.Parse("#f0f")),
            ("green", Color.Parse("#3f0")),
            ("white", Color.Parse("#fff")),
            ("orange", Color.Parse("#f90")),
        };

        public PotionFactory(Random rnd)
        {
            types = types.OrderBy(p => rnd.Next()).ToArray();
            BaseId = StackComponent.GenMergeId(types.Length);
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            Entity e = new Entity('?', "Potion", EEntityFlag.Pickable);

            var potionId = info.Rnd.Next(2);

            (var colorName, var color) = types[potionId];
            e.Symbol = new Symbol('?', color);

            e.Add(new StackComponent(1, potionId + BaseId));

            switch (potionId)
            {
                case 0: // Lesser healing potion
                    e.Add(new HealComponent(20, 0, typeof(DrinkMessage)));
                    break;
                case 1: // Greater healing potion
                    e.Add(new HealComponent(40, 0, typeof(DrinkMessage)));
                    break;
            }
            //TODO: Add randomized effects to the potion.

            //TODO: Work on identitying potions
            e.Name = $"{colorName} potion";

            return e;
        }
    }
}
