using System;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ai;
using System.Collections.Generic;

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
                var c2 = c as T;

                if (c2 != null)
                    yield return c2;
            }
        }

        public T GetOne<T>() where T : class, IComponent
        {
            foreach (var c in Components)
            {
                var c2 = c as T;

                if (c2 != null)
                    return c2;
            }
            return null;
        }

        internal void Add(IComponent component)
        {
            Components.Add(component);
        }

        public bool Event(Entity item, Entity target, EUseMessage message)
        {
            bool change = false;

            foreach (var c in item.Components)
                change |= c.Use(this, item, target, message);

            if (change)
                if (item.GetOne<StackComponent>()?.Consume() ?? false)
                    item.Destroy();

            return change;
        }

        public void Destroy()
        {
            EntityFlags |= EEntityFlag.Destroyed;
            OnDestroy?.Invoke(this);

            GetOne<Inventory>()?.Remove(this);
            Move?.Unbind(this);
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
