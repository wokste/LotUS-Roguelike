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

        public TerrainFlag Flags;
        public Symbol Symbol;

        public float Speed = 1;

        public AiActor Ai;
        public Attitude Attitude;

        public float LeftoverMove;

        public Action<Entity> OnDestroy;


        public Level Level { get; private set; }
        public Vec Pos;

        public Entity(char ascii, string name, EEntityFlag entityFlags)
        {
            Symbol = new Symbol(ascii, Color.White);
            Name = name;
            EntityFlags = entityFlags;
        }

        public Entity(char ascii, Color fc, string name, EEntityFlag entityFlags)
        {
            Symbol = new Symbol(ascii, fc);
            Name = name;
            EntityFlags = entityFlags;
        }

        public IEnumerable<T> Get<T>() where T : class, IComponent
        {
            foreach (var c in Components)
            {
                if (c is T c2)
                {
                    yield return c2;
                }
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
            // == Inventory ==
            var inv = GetOne<Inventory>();
            if (inv != null)
                foreach (var item in inv.Equipped)
                    if (item != null)
                        yield return item;

            // == Status effects ==
            // TODO: Add status effects.
        }

        public void Add(IComponent component)
        {
            Components.Add(component);
        }

        public void Destroy()
        {
            EntityFlags |= EEntityFlag.Destroyed;
            OnDestroy?.Invoke(this);

            SetLevel(null, Vec.Zero);
        }

        public (Entity,IWeapon) GetWeapon(Entity target)
        {
            IEnumerable<Entity> It() {
                var inventory = GetOne<Inventory>();
                yield return inventory?.Equipped[0];
                yield return inventory?.Equipped[1];
                yield return this;
            }

            foreach(var elem in It())
            {
                var comp = elem?.GetOne<IWeapon>();
                if (comp != null && comp.InRange(this, target))
                    return (elem, comp);
            }

            return (null, null);
        }

        public void SetLevel(Level newLevel, Vec newPos)
        {
            Level?.GetChunck(Pos).Remove(this);
            Level = null;

            if (newLevel != null)
            {
                var c = newLevel.GetChunck(newPos);
                c.Add(this);

                if (newLevel != null)
                {
                    Level = newLevel;
                    Pos = newPos;
                }
            }
        }

        public virtual bool Move(Entity self, Vec direction)
        {
            var newPosition = Pos;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!Level.InBoundary(newPosition))
                return false;

            // Terrain collisions
            if (!Level.HasFlag(newPosition, self.Flags))
                return false;

            // Monster collisions
            if (self.EntityFlags.HasFlag(EEntityFlag.Blocking))
                foreach (var c in Level.GetEntities(newPosition))
                    if (c.EntityFlags.HasFlag(EEntityFlag.Blocking))
                        return false;

            var oldChunk = Level.GetChunck(Pos);
            Pos = newPosition;
            var newChunk = Level.GetChunck(Pos);

            if (oldChunk != newChunk)
            {
                oldChunk.Remove(self);
                newChunk.Add(self);
            }

            self.GetOne<FieldOfView>()?.Update(this);

            return true;
        }
    }

    [Flags]
    public enum EEntityFlag
    {
        Blocking = 0x1, // Occupies a square
        Pickable = 0x2, // Can be held in the inventory.
        FixedPos = 0x4, // Nothing should be able to move this entity. E.g. Doors, fountains and stairs down shouldn't be affected by push-effects.
        IsPlayer = 0x8, // Only the player actor is a player. This is useful for confusion-like debuffs that effect either AI or UI.
        Destroyed = 0x10,

        // Teams are for AI. 
        TeamPlayer = 0x100, // Players, summons, etc
        TeamMonster = 0x200, // Most monsters aggressive to the player

        // Blessed and cursed
        Identified = 0x1000, // Set on true if item/monster is identified.
        Cursed = 0x2000,

        // Item usage flags
        Consumable = 0x10000,
        Throwable = 0x20000,
        Interactable = 0x40000,
    }
}
