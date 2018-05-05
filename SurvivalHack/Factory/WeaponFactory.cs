using System;
using HackConsole;

namespace SurvivalHack.Factory
{
    class WeaponFactory : IEntityFactory
    {
        (string name, Range damage, string tags)[] WeaponTypes = new(string name, Range damage, string tags)[]
        {
            ("Dagger", new Range("4-6"), "piercing" ),
            ("Shortsword", new Range("5-7"), "slashing, piercing" ),
            ("Longsword", new Range("6-9"), "shashing, piercing" ),
            ("Handaxe", new Range("6-8"), "slashing, hooking" ),
            ("Spear", new Range("5-9"), "piercing, range" ),
            ("Mace", new Range("5-7"), "bludgeoing" ),
        };

        public Entity Gen(EntityGenerationInfo info)
        {
            var e = new Entity();

            InitBase(info, e);

            if (info.Level != 0 && info.Rnd.NextDouble() < 0.2)
                Enchant(info, e);

            return e;
        }

        private void Enchant(EntityGenerationInfo info, Entity e)
        {
            var modifier = new Range(1, info.Level / 3 + 1).Rand(info.Rnd);

            e.Name = $"{e.Name} +{modifier}";

            foreach(var c in e.Get<ECM.AttackComponent>())
            {
                c.Damage += modifier;
            }
        }

        private void InitBase(EntityGenerationInfo info, Entity e)
        {
            e.Symbol.TextColor = Color.Parse("#859a9a");

            switch (new Range(1, 6).Rand(info.Rnd))
            {
                case 1:
                    e.Name = "Dagger";
                    e.Add(new ECM.AttackComponent(5, EDamageType.Piercing));
                    e.Symbol.Ascii = '|';
                    return;
                case 2:
                    e.Name = "Shortsword";
                    e.Add(new ECM.AttackComponent(6, EDamageType.Slashing | EDamageType.Piercing));
                    e.Symbol.Ascii = '|';
                    return;
                case 3:
                    e.Name = "Longsword";
                    e.Add(new ECM.AttackComponent(7, EDamageType.Slashing | EDamageType.Piercing));
                    e.Symbol.Ascii = '|';
                    return;
                case 4:
                    e.Name = "Handaxe";
                    e.Add(new ECM.AttackComponent(6, EDamageType.Slashing));
                    e.Symbol.Ascii = '|';
                    return;
                case 5:
                    e.Name = "Spear";
                    e.Add(new ECM.AttackComponent(7, EDamageType.Piercing));
                    e.Symbol.Ascii = '/';
                    return;
                case 6:
                    e.Name = "Mace";
                    e.Add(new ECM.AttackComponent(6, EDamageType.Bludgeoing));
                    e.Symbol.Ascii = '\\';
                    return;
            }
        }
    }
}
