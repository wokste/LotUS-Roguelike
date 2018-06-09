using System;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ai;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.Combat;

namespace SurvivalHack
{
    public class Entity
    {
        public String Name { get; set; }

        public EEntityFlag EntityFlags;

        public override string ToString() => Name;

        public List<IComponent> Components = new List<IComponent>();
        public MoveComponent Move;

        public TerrainFlag Flags;
        public Symbol Symbol;

        public float Speed = 1;

        public AiActor Ai;
        public Attitude Attitude;

        public float LeftoverMove;

        public Action<Entity> OnDestroy;

        public Entity(char ascii, string name, EEntityFlag entityFlags)
        {
            Symbol = new Symbol(ascii, Color.White);
            Name = name;
            EntityFlags = entityFlags;
        }

        public IEnumerable<T> Get<T>() where T : class, IComponent
        {
            foreach (var c in Components)
            {
                if (c is T c2)
                    yield return c2;
            }
        }

        public T GetOne<T>() where T : class, IComponent
        {
            foreach (var comp in Components)
            {
                if (comp is T compTypeT)
                    return compTypeT;
            }
            return null;
        }

        public IEnumerable<Entity> ListSubEntities()
        {
            yield return this;

            // == Inventory ==
            var inv = GetOne<Inventory>();
            if (inv != null)
                foreach (var item in inv.Equipped)
                    if (item != null)
                        yield return item;

            // == Status effects ==
            // TODO: Add status effects.
        }

        internal void Add(IComponent component)
        {
            Components.Add(component);
        }

        public bool Event(Entity item, Entity target, EUseMessage message, IEnumerable<UseFunc> functions = null)
        {
            var funcs = new List<UseFunc>(functions ?? Enumerable.Empty<UseFunc>());
            funcs.AddRange(item.Components.SelectMany(c => c.GetActions(message, EUseSource.This)));

            if (!funcs.Any(f => f.Order == EUseOrder.Event))
            {
                return false;
            }

            funcs.AddRange(target.Components.SelectMany(c => c.GetActions(message, EUseSource.Target)));
            funcs.AddRange(this.Components.SelectMany(c => c.GetActions(message, EUseSource.User)));

            if (funcs.Any(f => f.Order == EUseOrder.Interrupt))
            {
                return false;
            }

            foreach (var f in funcs.OrderBy(f => f.Order))
            {
                f.Action?.Invoke(this, item, target);
            }

            return true;
        }

        public void Destroy()
        {
            EntityFlags |= EEntityFlag.Destroyed;
            OnDestroy?.Invoke(this);

            Move?.Unbind(this);
        }

        internal (Entity Item, IWeapon IWeapon) GetWeapon(Entity target)
        {
            IEnumerable<(Entity Item, IWeapon IWeapon)> WeaponFilter(Entity e) {
                foreach (var w in e.Get<IWeapon>())
                    if (w.InRange(this, target))
                        yield return (e, w);
            }
            var pairList = ListSubEntities().SelectMany(WeaponFilter).OrderByDescending(p => p.IWeapon.WeaponPriority + Game.Rnd.NextDouble());
            return pairList.FirstOrDefault();
        }
    }

    [Flags]
    public enum EEntityFlag
    {
        Blocking = 0x1, // Occupies a square
        Pickable = 0x2, // Can be held in the inventory.
        FixedPos = 0x4, // Nothing should be able to move this entity. E.g. Doors, fountains and stairs down shouldn't be affected by push-effects.
        Identified = 0x8, // Set on true if item/monster is identified.
        IsPlayer = 0x10, // Only the player actor is a player. This is useful for confusion-like debuffs that effect either AI or UI.
        Destroyed = 0x20,

        // Teams are for AI. 
        TeamPlayer = 0x100, // Players, summons, etc
        TeamMonster = 0x200, // Most monsters aggressive to the player

    }
}
