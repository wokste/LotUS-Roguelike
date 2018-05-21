﻿using HackConsole;
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

        public bool Use(Entity user, Entity item, Entity target, EUseMessage msg)
        {
            if (Filter != msg)
                return false;

            user.GetOne<Combat.Damagable>().Heal(Restore, StatID);

            return true;
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

        public bool Use(Entity user, Entity item, Entity target, EUseMessage msg)
        {
            if (Filter != msg)
                return false;

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

            return true;
        }

        public string Describe() => $"Reveals the map";
    }
}