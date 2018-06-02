using HackConsole;
using System;
using System.Collections.Generic;

namespace SurvivalHack.ECM
{
    public class HealComponent : IComponent
    {
        public int Restore;
        public int StatID;
        public EUseMessage Filter { get; }

        public HealComponent(int restore, int statID, EUseMessage filter)
        {
            Restore = restore;
            StatID = statID;
            Filter = filter;
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source)
        {
            if (filter == Filter && source == EUseSource.This)
                yield return new UseFunc(Heal);
        }

        public void Heal(Entity user, Entity item, Entity target)
        {
            bool healed = user.GetOne<Combat.Damagable>().Heal(Restore, StatID);

            if (healed && user.EntityFlags.HasFlag(EEntityFlag.IsPlayer))
            {
                Message.Write($"You heal {Restore} HP", null, Color.Green);
                // TODO: Item identification
            }
        }

        public string Describe() => $"Heals {Restore} when {Filter}.";
    }

    public class MapRevealComponent : IComponent
    {
        public EUseMessage Filter { get; }
        public byte Flags;

        public MapRevealComponent(byte flags, EUseMessage filter)
        {
            Filter = filter;
            Flags = flags;
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source)
        {
            if (filter == Filter && source == EUseSource.This)
                yield return new UseFunc(Reveal);
        }

        public void Reveal(Entity user, Entity item, Entity target)
        {
            var FoV = user.GetOne<FieldOfView>();
            var Map = FoV.Map;

            // This function returns true if it can be seen from any direction.
            bool Reachable(Vec v)
            {
                for (int y = Math.Max(v.Y - 1, 0); y <= Math.Min(v.Y + 1, Map.Size.Y - 1); y++)
                    for (int x = Math.Max(v.X - 1, 0); x <= Math.Min(v.X + 1, Map.Size.X - 1); x++)
                        if (Map.HasFlag(new Vec(x, y), TerrainFlag.Sight))
                            return true;
                return false;
            }

            // Now update all tiles such that things become visible
            foreach (var v in Map.TileMap.Ids())
            {
                if (Reachable(v))
                    FoV.Visibility[v] |= Flags;
            }
        }

        public string Describe() => $"Reveals the map";
    }
}