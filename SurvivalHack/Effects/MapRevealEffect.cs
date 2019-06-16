using HackConsole;
using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    [XmlType("MapReveal")]
    public class MapRevealEffect : IEffect
    {
        [XmlAttribute]
        public EntityTarget UseOn { get; set; }

        [XmlAttribute]
        public int Radius { get; set; } = int.MaxValue;

        public enum RevealMethod
        {
            Terrain, TremorSense, FullKnowledge
        }

        [XmlAttribute]
        public RevealMethod Method { get; set; }

        public MapRevealEffect()
        {
        }

        public MapRevealEffect(RevealMethod method, int radius = int.MaxValue)
        {
            Method = method;
            Radius = radius;
        }

        public void RevealAround(FieldOfView fov, IShape shape, RevealMethod method, Func<Vec, bool> pred = null)
        {
            var Map = fov.Map;

            var flags = (method == RevealMethod.FullKnowledge) ? FieldOfView.FLAG_ALWAYSVISIBLE : FieldOfView.FLAG_DISCOVERED;

            // Returns whether it can be seen from any direction.
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
                if (Reachable(v) && (pred == null || pred(v)))
                    fov.Visibility[v] |= flags;
            }
        }
        
        public void Use(Entity instignator, Entity target, StringBuilder _)
        {
            var map = instignator.Level;
            var fov = instignator.GetOne<FieldOfView>();
            var circle = new Circle(instignator.Pos, Radius);

            Debug.Assert(fov != null);
            if (fov == null)
                return;

            switch (Method) {
                case RevealMethod.FullKnowledge:
                case RevealMethod.Terrain:
                    {
                        RevealAround(fov, circle, Method);
                        break;
                    }
                case RevealMethod.TremorSense:
                    {
                        foreach (var e in map.GetEntities(circle))
                            if (e.EntityFlags.HasFlag(EEntityFlag.TeamPlayer) || e.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                                RevealAround(fov, new Circle(e.Pos, 1), Method);
                    }
                    break;
            }
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 0; // AI's can't work with this component.
        }
    }
}
