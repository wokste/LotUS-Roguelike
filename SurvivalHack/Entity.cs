using System;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ai;
using System.Collections.Generic;

namespace SurvivalHack
{
    public class Entity : IDescriptionProvider
    {
        public String Name { get; set; }

        public EEntityFlag EntityFlags;

        public override string ToString() => Name;

        public Bar Health;
        public Bar Hunger;

        public List<IComponent> Components = new List<IComponent>();
        public MoveComponent Move;

        public TerrainFlag Flags;
        public Symbol Symbol;

        public bool Alive = true;

        public string Description { get; set; }

        public float Speed = 1;

        public AiActor Ai;
        public Attitude Attitude;

        public float LeftoverMove;

        public Action<Entity> OnDestroy;

        public Entity(char ascii, string name, EEntityFlag entityFlags)
        {
            Symbol.Ascii = ascii;
            Name = name;
            EntityFlags = entityFlags;
        }

        public void TakeDamage(int damage, EDamageType damageType)
        {
            Health.Current -= damage;

            if (Health.Current == 0)
            {
                Alive = false;
                OnDestroy?.Invoke(this);
                Move.Unbind(this);

                Message.Write($"{Name} died", Move?.Pos, Color.Red);
            }
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

        public bool UseItem(Entity item, EUseMessage message)
        {
            var ls = item.Get<IUsableComponent>();
            bool any = false;

            foreach (var c in ls)
            {
                any = true;
                c.Use(item, this, message);
            }

            if (any == false)
            {
                Message.Write("You can't use that, silly person.", null, Color.Red);
                return false;
            }

            if (item.GetOne<StackComponent>()?.Consume() ?? false)
                GetOne<Inventory>()?.Remove(item);

            return true;
        }

        internal void Attack(Entity enemy, Entity weapon)
        {
            foreach ( var attack in weapon.Get<ECM.IAttackComponent>())
            {
                attack.Attack(this, enemy);
            }
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

        // Teams are for AI. 
        TeamPlayer = 0x100, // Players, summons, etc
        TeamMonster = 0x200, // Most monsters aggressive to the player
    }
}
