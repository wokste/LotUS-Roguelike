using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Diagnostics;
using System.Linq;

namespace SurvivalHack.Combat
{
    [Flags]
    public enum EDamageType
    {
        Bludgeoing = 1,
        Piercing = 2,
        Slashing = 4,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }

    public enum EAttackMove
    {
        Projectile = 1,
        Piercing = 2,
        Slashing = 4,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }

    public enum EDamageLocation
    {
        Head,
        Body,
        LArm,
        RArm,
        Legs,
        Wings,
        Tail,
    }
}
