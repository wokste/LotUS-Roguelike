using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Effects
{
    public interface IEffect
    {
        string Describe();
    }

    public interface IEntityEffect : IEffect
    {
        EntityTarget Target { get; }

        bool Use(Entity instignator, Entity target, StringBuilder sb);
        float Efficiency(Entity instignator, Entity target);
    }

    public interface ITileEffect : IEffect
    {
        bool Use(Entity insignator, Grid<Tile> map, StringBuilder sb);
        float Efficiency(Entity instignator, Grid<Tile> map);
    }

    public interface IGlobalEffect : IEffect
    {
        bool Use(Entity insignator, StringBuilder sb);
        float Efficiency(Entity instignator);
    }

    [Flags]
    public enum EntityTarget {
        Self = 1,
        Inventory = 2,
        Others = 4,
        MultiTarget = 8,
    }
}
