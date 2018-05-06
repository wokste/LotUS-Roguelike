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

        RandomTable<string> BasePropabilities;

        public WeaponFactory() {
            BasePropabilities = RandomTable<string>.FromString("dagger:3,ssword:2,lsword:5,axe:3,spear:5,mace:2,sbow:3,lbow:5");
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            var tag = BasePropabilities.GetRand(info.Rnd);

            var e = GetBasic(tag);

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

        public Entity GetBasic(string tag)
        {
            switch (tag)
            {
                case "dagger":
                    {
                        var e = new Entity('|', "Dagger", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(5, EDamageType.Piercing));
                        e.Add(new EquippableComponent(ESlotType.Hand));
                        return e;
                    }
                case "ssword":
                    {
                        var e = new Entity('|', "Shortsword", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(6, EDamageType.Slashing | EDamageType.Piercing));
                        e.Add(new EquippableComponent(ESlotType.Hand));
                        return e;
                    }
                case "lsword":
                    {
                        var e = new Entity('|', "Longsword", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(7, EDamageType.Slashing | EDamageType.Piercing));
                        e.Add(new EquippableComponent(ESlotType.Hand));
                        return e;
                    }
                case "axe":
                    {
                        var e = new Entity('\\', "Handaxe", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(6, EDamageType.Slashing));
                        e.Add(new EquippableComponent(ESlotType.Hand));
                        return e;
                    }
                case "spear":
                    {
                        var e = new Entity('/', "Spear", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(7, EDamageType.Piercing));
                        e.Add(new EquippableComponent(ESlotType.Hand));
                        return e;
                    }
                case "mace":
                    {
                        var e = new Entity('\\', "Mace", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(6, EDamageType.Bludgeoing));
                        e.Add(new EquippableComponent(ESlotType.Hand));
                        return e;
                    }
                case "sbow":
                    {
                        var e = new Entity(')', "Shortbow", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(5, EDamageType.Piercing) { Range = 20 });
                        e.Add(new EquippableComponent(ESlotType.Ranged));
                        //TODO: require ammo
                        return e;
                    }
                case "lbow":
                    {
                        var e = new Entity(')', "Longbow", EEntityFlag.Pickable);
                        e.Add(new ECM.AttackComponent(6, EDamageType.Piercing) { Range = 50 });
                        e.Add(new EquippableComponent(ESlotType.Ranged));
                        //TODO: require ammo
                        return e;
                    }
            }
            throw new ArgumentException($"Tag {tag} not found");
        }
    }

}
