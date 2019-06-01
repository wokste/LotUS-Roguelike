using HackConsole;
using System;

namespace SurvivalHack.ECM
{
    public class HealComponent : Component
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

        public override void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (MessageType.IsAssignableFrom(message.GetType()) && source == EUseSource.Item)
                message.OnEvent += Heal;
        }

        public void Heal(BaseEvent msg)
        {
            Eventing.On(new HealEvent(msg, Restore, StatID), msg);
        }

        public override string Describe() => $"Heals {Restore} when used.";
    }

    public class MapRevealComponent : Component
    {
        public Type MessageType { get; }
        private readonly byte FovFlags;
        public int Radius;

        public enum RevealMethod {
            Walls, All, // Maybe add heat (lava), movement (non-flying creatures), etc.
        }

        readonly RevealMethod Method;

        public MapRevealComponent(Type messageType, byte fovFlags, RevealMethod method, int radius)
        {
            MessageType = messageType;
            FovFlags = fovFlags;
            Method = method;
            Radius = radius;
        }

        public override void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (MessageType.IsAssignableFrom(message.GetType()) && source == EUseSource.Item)
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
                    {
                        var tile = Map.GetTile(new Vec(x, y));
                        if (!tile.Solid || !tile.BlockSight)
                            return true;
                    }
                return false;
            }



            // Now update all tiles such that things become visible
            foreach (var v in Map.TileMap.Ids())
            {
                if (Reachable(v) && Map.GetTile(v).Solid)
                    FoV.Visibility[v] |= FovFlags;
            }
        }

        public override string Describe() => $"Reveals the map";
    }
}