using HackConsole;
using System;

namespace SurvivalHack.Factory
{
    public class ItemFactory : IEntityFactory
    {
        private IEntityFactory _weaponFactory = new WeaponFactory();
        private IEntityFactory _potionFactory;

        public ItemFactory(Random rnd) {
            _potionFactory = new PotionFactory(rnd);
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            var d100 = new Range(1, 100);
            var f = d100.Rand(info.Rnd);

            if (f <= 20)
                return _weaponFactory.Gen(info);
            else
                return _potionFactory.Gen(info);
        }

        public static Entity Get(string tag)
        {
            switch (tag)
            {
                default:
                    throw new ArgumentException("unknown tag " + tag);
            }
        }

    }
}
