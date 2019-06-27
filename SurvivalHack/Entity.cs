using System;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ai;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SurvivalHack
{
    public class Entity
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public EEntityFlag EntityFlags { get; set; }
        public override string ToString() => Name;

        [XmlAnyElement]
        public ComponentList Components { get; set; } = new ComponentList();

        [XmlIgnore]
        public TileGlyph Glyph { get; private set; }


        [XmlAttribute("Glyph")]
        public string GlyphXmlString { get => Glyph.ToString(); set => Glyph = new TileGlyph(value); }

        [XmlAttribute]
        public float Speed { get; set; } = 1;

        [XmlElement] // TODO
        public AiActor Ai { get; set; }

        [XmlElement] // TODO
        public Attitude Attitude { get; set; }

        [XmlIgnore]
        public float LeftoverMove;
        [XmlIgnore]
        public Action<Entity> OnDestroy;

        [XmlIgnore]
        public Level Level { get; private set; }
        [XmlIgnore]
        public Vec Pos;
        [XmlIgnore]
        public Vec? LastSeenPos;

        public Entity()
        {
        }

        public Entity(TileGlyph glyph, string name, EEntityFlag entityFlags)
        {
            Glyph = glyph;
            Name = name;
            EntityFlags = entityFlags;
        }

        public IEnumerable<T> Get<T>() where T : class, IComponent
        {
            foreach (var c in Components)
                if (c is T c2)
                    yield return c2;

        }

        public IEnumerable<T> GetNested<T>(IList<T> list = null) where T : class, IComponent
        {
            list = list ?? new List<T>();
            foreach (var c in Components)
            {
                if (c is T c2)
                {
                    list.Add(c2);
                }
                else if (c is INestedComponent nc)
                {
                    nc.GetNested(list);
                }
            }
            return list;
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
