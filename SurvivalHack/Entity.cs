using System;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ai;
using System.Collections.Generic;
using SurvivalHack.Combat;
using System.Diagnostics;

namespace SurvivalHack
{
    public class Entity
    {
        public string Name { get; set; }

        public EEntityFlag EntityFlags;
        public override string ToString() => Name;
        public List<IComponent> Components = new List<IComponent>();

        public TileGlyph Glyph;
        public float Speed = 1;

        public AiActor Ai;
        public Attitude Attitude;

        public float LeftoverMove;
        public Action<Entity> OnDestroy;

        public Level Level { get; private set; }
        public Vec Pos;
        public Vec? LastSeenPos;

        public Entity(TileGlyph glyph, string name, EEntityFlag entityFlags)
        {
            Glyph = glyph;
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
                foreach (var slot in inv.Slots)
                    if (slot.Item != null)
                        yield return slot.Item;

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

            if (!EntityFlags.HasFlag(EEntityFlag.IsPlayer))
                SetLevel(null, Vec.Zero);
        }

        public (Entity,IWeapon) GetWeapon(Entity target)
        {
            IEnumerable<Entity> It() {
                var inventory = GetOne<Inventory>();
                yield return inventory?.Slots[0].Item;
                yield return inventory?.Slots[1].Item;
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

                Level = newLevel;
                Pos = newPos;

                GetOne<FieldOfView>()?.Update(this);
            }
        }

        public bool TryMove(Vec direction)
        {
            Debug.Assert(Level != null);

            var newPosition = Pos + direction;

            return TrySetPos(newPosition);
        }

        public bool TrySetPos(Vec newPosition)
        {
            // You cannot walk of the edge of map
            if (!Level.InBoundary(newPosition))
                return false;

            // Terrain collisions
            if (Level.GetTile(newPosition).Solid)
                return false;

            // Monster collisions
            if (EntityFlags.HasFlag(EEntityFlag.Blocking))
                foreach (var c in Level.GetEntities(newPosition))
                    if (c.EntityFlags.HasFlag(EEntityFlag.Blocking))
                        return false;

            var oldChunk = Level.GetChunck(Pos);
            Pos = newPosition;
            var newChunk = Level.GetChunck(Pos);

            if (oldChunk != newChunk)
            {
                oldChunk.Remove(this);
                newChunk.Add(this);
            }

            GetOne<FieldOfView>()?.Update(this);

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
