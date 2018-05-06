using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Factory
{
    class PotionFactory : IEntityFactory
    {
        (string, Color)[] types = new(string, Color)[]{
            ("Red", Color.Parse("#f00")),
            ("blue", Color.Parse("#36f")),
            ("aqua", Color.Parse("#0fc")),
            ("yellow", Color.Parse("#cf0")),
            ("purple", Color.Parse("#f0f")),
            ("green", Color.Parse("#3f0")),
            ("white", Color.Parse("#fff")),
            ("orange", Color.Parse("#f90")),
        };

        public Entity Gen(EntityGenerationInfo info)
        {
            Entity e = new Entity('?', "Potion", EEntityFlag.Pickable);

            var colorId = info.Rnd.Next(types.Length);

            (var colorName, var color) = types[colorId];
            e.Symbol = new Symbol('?', color);
            e.Add(new StackComponent(1, colorId));

            //TODO: Add randomized effects to the potion.
            e.Add(new HealComponent(5, 0, EUseMessage.Drink));

            //TODO: Work on identitying potions
            e.Name = $"{colorName} potion | Health potion";

            return e;
        }
    }
}
