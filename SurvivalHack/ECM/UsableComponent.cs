using HackConsole;
using System;
using System.Collections.Generic;

namespace SurvivalHack.ECM
{
    public class HealComponent : IComponent
    {
        public int Restore;
        public int StatID;
        public Type MessageType { get; }

        public HealComponent(int restore, int statID, Type messageType)
        {
            Restore = restore;
            StatID = statID;
            MessageType = messageType;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (MessageType.IsAssignableFrom(message.GetType()) && source == EUseSource.This)
                message.OnEvent += Heal;
        }

        public void Heal(BaseEvent msg)
        {
            Eventing.On(new HealEvent(msg, Restore, StatID), msg);
        }

        public string Describe() => $"Heals {Restore} when used.";
    }

    public class MapRevealComponent : IComponent
    {
        public Type MessageType { get; }
        public byte Flags;

        public MapRevealComponent(byte flags, Type messageType)
        {
            MessageType = messageType;
            Flags = flags;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (MessageType.IsAssignableFrom(message.GetType()) && source == EUseSource.This)
                message.OnEvent += Reveal;
        }

        public void Reveal(BaseEvent msg)
        {
            var FoV = msg.User.GetOne<FieldOfView>();
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