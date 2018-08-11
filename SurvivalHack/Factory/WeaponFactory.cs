using System;
using HackConsole;

namespace SurvivalHack.Factory
{
    public class WeaponFactory : IEntityFactory
    {
        RandomTable<string> BasePropabilities;

        public WeaponFactory() {
            BasePropabilities = RandomTable<string>.FromString("dagger:3,ssword:2,lsword:5,axe:3,spear:5,mace:2,wshield:3,ishield:1,sbow:13,lbow:21,armour_leather:3,armour_chain:2,armour_plate:1,helmet:5,gloves:2,gauntlets:1,boots:2,ring:1,cloak:1,amulet:1,@resistance:3");
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            var tag = BasePropabilities.GetRand(info.Rnd);

            var e = GetBasic(tag);

            Morph(info, e);

            return e;
        }

        private void Morph(EntityGenerationInfo info, Entity e)
        {
            var roll = info.Rnd.Next(1,3);

            if (roll == 1)
            {
                e.Name = $"Cursed {e.Name}";
                e.EntityFlags |= EEntityFlag.Cursed;
            }
        }

        public Entity GetBasic(string tag)
        {
            switch (tag)
            {
                case "dagger":
                    {
                        var e = new Entity('\\', Colour.Gray, "Dagger", EEntityFlag.Pickable | EEntityFlag.Throwable);
                        e.Add(new Combat.MeleeWeapon(5, Combat.EAttackMove.Close, Combat.EDamageType.Piercing));
                        e.Add(new Combat.Blockable(0.2f, Combat.EAttackState.Parried));
                        return e;
                    }
                case "ssword":
                    {
                        var e = new Entity('\\', Colour.Gray, "Shortsword", EEntityFlag.Pickable);
                        e.Add(new Combat.MeleeWeapon(6, Combat.EAttackMove.Swing, Combat.EDamageType.Slashing | Combat.EDamageType.Piercing));
                        e.Add(new Combat.Blockable(0.2f, Combat.EAttackState.Parried));
                        return e;
                    }
                case "lsword":
                    {
                        var e = new Entity('\\', Colour.Gray, "Longsword", EEntityFlag.Pickable);
                        e.Add(new Combat.MeleeWeapon(7, Combat.EAttackMove.Swing, Combat.EDamageType.Slashing | Combat.EDamageType.Piercing));
                        e.Add(new Combat.Blockable(0.2f, Combat.EAttackState.Parried));
                        return e;
                    }
                case "axe":
                    {
                        var e = new Entity('\\', Colour.Orange, "Handaxe", EEntityFlag.Pickable | EEntityFlag.Throwable);
                        e.Add(new Combat.MeleeWeapon(6, Combat.EAttackMove.Swing, Combat.EDamageType.Slashing));
                        e.Add(new Combat.Blockable(0.1f, Combat.EAttackState.Parried));
                        return e;
                    }
                case "spear":
                    {
                        var e = new Entity('\\', Colour.Orange, "Spear", EEntityFlag.Pickable | EEntityFlag.Throwable);
                        e.Add(new Combat.MeleeWeapon(7, Combat.EAttackMove.Thrust, Combat.EDamageType.Piercing));
                        return e;
                    }
                case "mace":
                    {
                        var e = new Entity('\\', Colour.Gray, "Mace", EEntityFlag.Pickable);
                        e.Add(new Combat.MeleeWeapon(6, Combat.EAttackMove.Swing, Combat.EDamageType.Bludgeoing));
                        return e;
                    }
                case "wshield":
                    {
                        var e = new Entity('[', Colour.Orange, "Wooden Shield", EEntityFlag.Pickable);
                        e.Add(new Combat.Blockable(0.3f, Combat.EAttackState.Blocked));
                        return e;
                    }
                case "ishield":
                    {
                        var e = new Entity('[', Colour.Gray, "Iron Shield", EEntityFlag.Pickable);
                        e.Add(new Combat.Blockable(0.3f, Combat.EAttackState.Blocked));
                        return e;
                    }
                case "sbow":
                    {
                        var e = new Entity(')', Colour.Orange, "Shortbow", EEntityFlag.Pickable);
                        e.Add(new Combat.RangedWeapon(5, Combat.EDamageType.Piercing, 20));
                        //TODO: require ammo
                        return e;
                    }
                case "lbow":
                    {
                        var e = new Entity(')', Colour.Orange, "Longbow", EEntityFlag.Pickable);
                        e.Add(new Combat.RangedWeapon(6, Combat.EDamageType.Piercing, 50 ));
                        //TODO: require ammo
                        return e;
                    }
                case "armour_leather":
                    {
                        var e = new Entity('^', Colour.Orange, "Leather Armour", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.AllBody, 2, 0.05f));
                        return e;
                    }
                case "armour_chain":
                    {
                        var e = new Entity('^', Colour.Gray, "Chainmail Armour", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.AllBody, 3, 0.05f));
                        return e;
                    }
                case "armour_plate":
                    {
                        var e = new Entity('^', Colour.White, "Plate Armour", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.AllBody, 4, 0.05f));
                        return e;
                    }
                case "helmet":
                    {
                        var e = new Entity('^', Colour.Cyan, "Helmet", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.Head, 3, 0.1f));
                        return e;
                    }

                case "boots":
                    {
                        var e = new Entity('&', Colour.Orange, "Boots", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.Feet, 2, 0.1f));
                        return e;
                    }
                case "gloves":
                    {
                        var e = new Entity('&', Colour.White, "Gloves", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.Hands, 2, 0.1f));
                        return e;
                    }
                case "gauntlets":
                    {
                        var e = new Entity('&', Colour.Gray, "Gauntlets", EEntityFlag.Pickable);
                        e.Add(new Combat.Armour(Combat.EDamageLocation.Hands, 4, 0.1f));
                        return e;
                    }
                case "ring":
                    {
                        var e = new Entity('*', Colour.Yellow, "Ring", EEntityFlag.Pickable);
                        e.Add(new Equippable(ESlotType.Ring));
                        return e;
                    }
                case "cloak":
                    {
                        var e = new Entity(']', Colour.Gray, "Cloak", EEntityFlag.Pickable);
                        e.Add(new Equippable(ESlotType.Neck));
                        return e;
                    }
                case "amulet":
                    {
                        var e = new Entity('*', Colour.Cyan, "Amulet", EEntityFlag.Pickable);
                        e.Add(new Equippable(ESlotType.Neck));
                        return e;
                    }
                case "@resistance":
                    {
                        var resistanceTypes = new(Combat.EDamageType Damage, string RandomTable)[] { ( Combat.EDamageType.Fire, "ring:5,amulet:1" ), ( Combat.EDamageType.Poison, "ring:5,amulet:1" ), (Combat.EDamageType.Ice, "ring:5,cloak:2") };
                        (var DamageType, var RandomTableText) = resistanceTypes[Game.Rnd.Next(resistanceTypes.Length)];

                        var e = GetBasic(RandomTable<string>.FromString(RandomTableText).GetRand(Game.Rnd));

                        var resistanceLevels = new(string Name, float Mult)[] { ("resistance", 0.5f), ("immunity", 0f) };
                        var (Name, Mult) = resistanceLevels[0];

                        e.Name = $"{e.Name} of {DamageType} {Name}";
                        e.Add(new Combat.ElementalResistance(DamageType, Mult));
                        return e;
                    }
            }
            throw new ArgumentException($"Tag {tag} not found");
        }
    }

}
