﻿using System;
using HackConsole;
using SurvivalHack.Combat;

namespace SurvivalHack.Factory
{
    public class WeaponFactory : IEntityFactory
    {
        readonly RandomTable<string> BasePropabilities;

        public WeaponFactory() {
            BasePropabilities = RandomTable<string>.FromString("dagger:3,ssword:2,lsword:5,axe:3,spear:5,mace:2,wshield:3,ishield:1,sbow:13,lbow:21,armour_leather:3,armour_chain:2,armour_plate:1,helmet:5,gloves:2,boots:2,ring:1,cloak:1,amulet:1,@resistance:3");
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
                        var e = new Entity(new TileGlyph(1,12), "Dagger", EEntityFlag.Pickable | EEntityFlag.Throwable);
                        e.Add(new MeleeWeapon(5, EAttackMove.Close, EDamageType.Piercing));
                        e.Add(new Blockable(0.2f, EAttackResult.Parried));
                        return e;
                    }
                case "ssword":
                    {
                        var e = new Entity(new TileGlyph(2, 12), "Shortsword", EEntityFlag.Pickable);
                        e.Add(new MeleeWeapon(6, EAttackMove.Swing, EDamageType.Slashing | EDamageType.Piercing));
                        e.Add(new Blockable(0.2f, EAttackResult.Parried));
                        return e;
                    }
                case "lsword":
                    {
                        var e = new Entity(new TileGlyph(5, 12), "Longsword", EEntityFlag.Pickable);
                        e.Add(new MeleeWeapon(7, EAttackMove.Swing, EDamageType.Slashing | EDamageType.Piercing));
                        e.Add(new Blockable(0.2f, EAttackResult.Parried));
                        return e;
                    }
                case "axe":
                    {
                        var e = new Entity(new TileGlyph(7, 12),"Handaxe", EEntityFlag.Pickable | EEntityFlag.Throwable);
                        e.Add(new SweepWeapon(new Damage(6, EDamageType.Slashing, EAttackMove.Swing)));
                        e.Add(new Blockable(0.1f, EAttackResult.Parried));
                        return e;
                    }
                case "spear":
                    {
                        var e = new Entity(new TileGlyph(13, 12), "Spear", EEntityFlag.Pickable | EEntityFlag.Throwable);
                        e.Add(new MeleeWeapon(7, Combat.EAttackMove.Thrust, Combat.EDamageType.Piercing));
                        return e;
                    }
                case "mace":
                    {
                        var e = new Entity(new TileGlyph(9, 12), "Mace", EEntityFlag.Pickable);
                        e.Add(new MeleeWeapon(6, EAttackMove.Swing, EDamageType.Bludgeoing));
                        return e;
                    }
                case "wshield":
                    {
                        var e = new Entity(new TileGlyph(13, 13), "Wooden Shield", EEntityFlag.Pickable);
                        e.Add(new Blockable(0.3f, EAttackResult.Blocked));
                        return e;
                    }
                case "ishield":
                    {
                        var e = new Entity(new TileGlyph(14, 13), "Iron Shield", EEntityFlag.Pickable);
                        e.Add(new Blockable(0.3f, EAttackResult.Blocked));
                        return e;
                    }
                case "sbow":
                    {
                        var e = new Entity(new TileGlyph(16, 12), "Shortbow", EEntityFlag.Pickable);
                        e.Add(new RangedWeapon(5, EDamageType.Piercing, 20));
                        //TODO: require ammo
                        return e;
                    }
                case "lbow":
                    {
                        var e = new Entity(new TileGlyph(17, 12), "Longbow", EEntityFlag.Pickable);
                        e.Add(new RangedWeapon(6, EDamageType.Piercing, 50 ));
                        //TODO: require ammo
                        return e;
                    }
                case "armour_leather":
                    {
                        var e = new Entity(new TileGlyph(0, 13), "Leather Armour", EEntityFlag.Pickable);
                        e.Add(new Armor(2, ESlotType.Body));
                        return e;
                    }
                case "armour_chain":
                    {
                        var e = new Entity(new TileGlyph(1, 13), "Chainmail Armour", EEntityFlag.Pickable);
                        e.Add(new Armor(3, ESlotType.Body));
                        return e;
                    }
                case "armour_plate":
                    {
                        var e = new Entity(new TileGlyph(2, 13), "Plate Armour", EEntityFlag.Pickable);
                        e.Add(new Armor(4, ESlotType.Body));
                        return e;
                    }
                case "helmet":
                    {
                        var e = new Entity(new TileGlyph(10, 13), "Helmet", EEntityFlag.Pickable);
                        e.Add(new Armor(3, ESlotType.Head));
                        return e;
                    }

                case "boots":
                    {
                        var e = new Entity(new TileGlyph(5, 13), "Boots", EEntityFlag.Pickable);
                        return e;
                    }
                case "gloves":
                    {
                        var e = new Entity(new TileGlyph(6, 13), "Gloves", EEntityFlag.Pickable);
                        return e;
                    }
                case "ring":
                    {
                        var e = new Entity(new TileGlyph(0, 16), "Ring", EEntityFlag.Pickable);
                        e.Add(new Equippable(ESlotType.Ring));
                        return e;
                    }
                case "cloak":
                    {
                        var e = new Entity(new TileGlyph(4, 13), "Cloak", EEntityFlag.Pickable);
                        e.Add(new Equippable(ESlotType.Neck));
                        return e;
                    }
                case "amulet":
                    {
                        var e = new Entity(new TileGlyph(0, 14), "Amulet", EEntityFlag.Pickable);
                        e.Add(new Equippable(ESlotType.Neck));
                        return e;
                    }
                case "@resistance":
                    {
                        var resistanceTypes = new(EDamageType Damage, string RandomTable)[] { (EDamageType.Fire, "ring:5,amulet:1" ), (EDamageType.Poison, "ring:5,amulet:1" ), (EDamageType.Ice, "ring:5,cloak:2") };
                        (var DamageType, var RandomTableText) = resistanceTypes[Game.Rnd.Next(resistanceTypes.Length)];

                        var e = GetBasic(RandomTable<string>.FromString(RandomTableText).GetRand(Game.Rnd));

                        var resistanceLevels = new(string Name, float Mult)[] { ("resistance", 0.5f), ("immunity", 0f) };
                        var (Name, Mult) = resistanceLevels[0];

                        e.Name = $"{e.Name} of {DamageType} {Name}";
                        e.Add(new ElementalResistance(DamageType, Mult));
                        return e;
                    }
            }
            throw new ArgumentException($"Tag {tag} not found");
        }
    }

}
