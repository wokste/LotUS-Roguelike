using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Effects;
using System;
using System.Diagnostics;
using System.Linq;

namespace SurvivalHack.Factory
{
    public class PotionFactory : IEntityFactory
    {
        public Potion[] _potions;

        public PotionFactory(Random rnd)
        {
            _potions = MakePotions();
            var icons = MakeNames().OrderBy(p => rnd.Next()).ToArray();

            Debug.Assert(_potions.Length <= icons.Length);

            for (int i = 0; i < _potions.Length; ++i)
            {
                _potions[i].UnidentifiedName = icons[i].Name;
                _potions[i].Glyph = icons[i].Glyph;
            }
        }

        Potion[] MakePotions() {
            return new Potion[] {
                new Potion("Lesser healing potion", new[] { new Heal(20, 0, EntityTarget.Self | EntityTarget.Others) }),
                new Potion("Greater healing potion", new[] { new Heal(40, 0, EntityTarget.Self | EntityTarget.Others) }),
                new Potion("Mana potion", new[] { new Heal(10, 1, EntityTarget.Self | EntityTarget.Others) }),
                new Potion("Claivorance", new IEffect[] {new MapReveal(MapReveal.RevealMethod.Terrain, 15)}),
            };
        }

        (TileGlyph Glyph, string Article, string Name)[] MakeNames()
        {
            return new(TileGlyph, string, string)[]{
                (new TileGlyph(0,15), "a", "red potion"),
                (new TileGlyph(1,15), "a", "pink potion"),
                (new TileGlyph(2,15), "an", "orange potion"),
                (new TileGlyph(3,15), "a", "yellow potion"),
                (new TileGlyph(4,15), "a", "grassy potion"),
                (new TileGlyph(5,15), "a", "mouldy potion"),
                (new TileGlyph(6,15), "a", "cyan potion"),
                (new TileGlyph(7,15), "a", "light blue potion"),
                (new TileGlyph(8,15), "a", "dark blue potion"),
                (new TileGlyph(16,15), "an", "oily potion"),
                (new TileGlyph(17,15), "a", "watery potion"),
                (new TileGlyph(18,15), "a", "dark potion"),
                (new TileGlyph(19,15), "a", "golden potion"),
                (new TileGlyph(20,15), "a", "muddy potion"),
                (new TileGlyph(22,15), "a", "gray potion"),
                (new TileGlyph(23,15), "a", "milky potion"),
            };
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            var potionId = info.Rnd.Next(_potions.Length);
            var potion = _potions[potionId];

            Entity e = new Entity(potion.Glyph, potion.IdentifiedName, EEntityFlag.Pickable | EEntityFlag.Consumable | EEntityFlag.Throwable);

            e.Add(new StackComponent(1, potion));
            e.Add(potion);

            return e;
        }
    }
}
